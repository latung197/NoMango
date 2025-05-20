using PlastMB.Application.CustomModels;
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.Others;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.Enum;
using PlastMB.Application.Interface;
using PlastMB.Domain.Entity;
using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Constants;
using PlastMB.Infrastructure.ContextAccessors;
using PlastMB.Utils;
using PlastMB.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlastMB.Application.Services
{
    public class MstUserServiceImpl : IMstUserService
    {
        #region Properties
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        #endregion
        #region Constructor
        public MstUserServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
        }
        #endregion
        #region Search
        public async Task<GenericResponseResult<AuthorizedUser>> SearchUser(MstUserSearchImpl condition, bool blnExport = false)
        {
            var iqResult = from u in _repo.MstUser.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.Username) || u.user_name.ToLower().Contains(condition.Username.ToLower()))
                           && (string.IsNullOrEmpty(condition.Email) || u.email.ToLower().Contains(condition.Email.ToLower()))
                           && (string.IsNullOrEmpty(condition.Fullname) || u.full_name.ToLower().Contains(condition.Fullname.ToLower()))
                           && (condition.Enable == u.enable_fl)
                           && (condition.Role == -1 || u.auth_fl.Contains(condition.Role.ToString()))
                           && u.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby u.user_id descending, (u.UpdateTime ?? u.CreateTime) descending
                           select _mapper.Map<MstUserDto>(u);

            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<AuthorizedUser>();
            }

            var lstData = await iqResult.ToListAsync();
            var lstResult = new List<AuthorizedUser>();
            foreach (var item in lstData){
                var author = new AuthorizedUser();
                author.UserId = item.user_id;
                author.Username = item.user_name;
                author.Fullname = item.full_name;
                author.Employeecode = item.employee_code;
                author.Email = item.email;
                author.Token = string.Empty;
                author.Role = string.IsNullOrEmpty(item.auth_fl) ? new List<int>() : item.auth_fl.Split(",").Select(x => int.Parse(x)).ToList();
                lstResult.Add(author);
            }
            return new GenericResponseResult<AuthorizedUser>(lstResult);
        }
        public async Task<ServiceResult> GetUserById(int id)
        {
            //Chỉ admin mới có quyền
            //if (!_userPrincipalService.Roles.Contains((int)EnumRole.Role.Admin)) return new ServiceResultError("Bạn không có quyền thực hiện thao tác này!");

            var entity = await _repo.MstUser.GetAsync(id);
            if (entity is null) return new ServiceResultError("Nhân viên không tồn tại");
            if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Nhân viên không hợp lệ!");
            var dto = _mapper.Map<MstUserDto>(entity);

            if (string.IsNullOrEmpty(dto.auth_fl))
                dto.Role = new List<int>();
            else
                dto.Role = dto.auth_fl.Split(",").Select(x => int.Parse(x)).ToList();
            //Admin need original password
            dto.password = StringUtils.Decrypt(entity.password);
            return new ServiceResultSuccess("Lấy dữ liệu thành công!", dto);
        }
        #endregion
        #region CRUD
        /// <summary>
        /// Update ngày 17/01/2024
        /// Thùy xinh đẹp yêu cầu phải chỉ rõ multi message với các field lỗi
        /// Tạo list các lỗi. Mỗi lần valid sẽ add lỗi vào
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ServiceResult> InsertUser(MstUserDto dto)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(dto.user_name) || string.IsNullOrEmpty(dto.user_name.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không được để trống!", });

                if (string.IsNullOrEmpty(dto.full_name) || string.IsNullOrEmpty(dto.full_name.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên nhân viên không được để trống!", });

                if (string.IsNullOrEmpty(dto.employee_code) || string.IsNullOrEmpty(dto.employee_code.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.employee_code), message = "Mã nhân viên không được để trống!", });

                if (!string.IsNullOrEmpty(dto.email))
                {
                    if (!StringUtils.IsValidEmail(dto.email))
                        lstErr.Add(new { field = nameof(MstUserDto.email), message = "Email không đúng định dạng!", });

                    var emailExist = await _repo.MstUser.AnyAsync(x => x.email.ToLower().Equals(dto.email.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (emailExist)
                        lstErr.Add(new { field = nameof(MstUserDto.email), message = "Email đã tồn tại!", });
                }

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                var userExist = await _repo.MstUser.AnyAsync(x => x.user_name.ToLower().Equals(dto.user_name.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (userExist)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập đã tồn tại!", });

                var employeeCodeExist = await _repo.MstUser.AnyAsync(x => x.employee_code.ToLower().Equals(dto.employee_code.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (employeeCodeExist)
                    lstErr.Add(new { field = nameof(MstUserDto.employee_code), message = "Mã nhân viên đã tồn tại!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                var entity = _mapper.Map<MstUser>(dto);
                entity.add_dt = DateTime.Now;

                string pass = _configuration["DefaultPassword"];

                if (!string.IsNullOrEmpty(dto.password))
                    pass = dto.password;

                entity.password = StringUtils.Encrypt(pass);
                entity.ValidFlg = (int)EnumCommon.Status.Valid;
                if (dto.Role is null || dto.Role.Count == 0)
                    entity.auth_fl = string.Empty;
                else
                    entity.auth_fl = string.Join(",", dto.Role.Select(x => x.ToString()));

                await _repo.MstUser.InsertAsync(entity);
                await _repo.SaveAync();

                var returnData = new Login { Username = dto.user_name, Password = pass };
                return new ServiceResultSuccess($"Thêm nhân viên thành công!", returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                //lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Lỗi khi thêm user: " + ex.Message });
                //return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);
                return new ServiceResultError($"Đã có lỗi xảy ra: {ex.Message}");
            }
        }
        /// <summary>
        /// Update ngày 17/01/2024
        /// Thùy xinh đẹp yêu cầu phải chỉ rõ multi message với các field lỗi
        /// Tạo list các lỗi. Mỗi lần valid sẽ add lỗi vào
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdateUser(MstUserDto dto)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(dto.user_name) || string.IsNullOrEmpty(dto.user_name.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không được để trống!", });

                if (string.IsNullOrEmpty(dto.full_name) || string.IsNullOrEmpty(dto.full_name.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên nhân viên không được để trống!", });

                if (string.IsNullOrEmpty(dto.employee_code) || string.IsNullOrEmpty(dto.employee_code.Trim()))
                    lstErr.Add(new { field = nameof(MstUserDto.employee_code), message = "Mã nhân viên không được để trống!", });

                if (!string.IsNullOrEmpty(dto.email))
                {
                    if (!StringUtils.IsValidEmail(dto.email))
                        lstErr.Add(new { field = nameof(MstUserDto.email), message = "Email không đúng định dạng!", });

                    if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                    var emailExist = await _repo.MstUser.AnyAsync(x => x.email.ToLower().Equals(dto.email.ToLower()) && x.user_id != dto.user_id && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (emailExist)
                        lstErr.Add(new { field = nameof(MstUserDto.email), message = "Email đã tồn tại!", });
                }

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //Check tồn tại bản ghi trong DB
                var entity = await _repo.MstUser.GetAsync(dto.user_id);
                if (entity is null)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không tồn tại" });

                if (entity is not null && entity.ValidFlg != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không hợp lệ" });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //Check trùng username trong database
                var existUser = await _repo.MstUser.AnyAsync(x => x.user_name.ToLower().Equals(dto.user_name.ToLower()) && x.user_id != dto.user_id && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (existUser)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập đã tồn tại!", });
                
                //Check trùng mã nhân viên trong database
                var employeeCodeExist = await _repo.MstUser.AnyAsync(x => x.employee_code.ToLower().Equals(dto.employee_code.ToLower()) && x.user_id != dto.user_id && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (employeeCodeExist)
                    lstErr.Add(new { field = nameof(MstUserDto.employee_code), message = "Mã nhân viên đã tồn tại!", });


                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                string pass = _configuration["DefaultPassword"];

                if (!string.IsNullOrEmpty(dto.password))
                    pass = dto.password;

                entity.user_name = dto.user_name;
                entity.enable_fl = dto.enable_fl;
                entity.email = dto.email;
                entity.auth_fl = dto.auth_fl;
                entity.full_name = dto.full_name;
                entity.employee_code = dto.employee_code;
                entity.Gender = dto.Gender;
                entity.upd_dt = DateTime.Now;

                if (dto.Role is null || dto.Role.Count == 0)
                    entity.auth_fl = string.Empty;
                else
                    entity.auth_fl = string.Join(",", dto.Role.Select(x => x.ToString()));
                entity.password = StringUtils.Encrypt(pass);
                entity.ValidFlg = (int)EnumCommon.Status.Valid;
                await _repo.MstUser.UpdateAsync(entity);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Cập nhật nhân viên thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Lỗi khi cập nhật user: " + ex.Message });
                return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);
            }
        }
        /// <summary>
        /// Update ngày 17/01/2024
        /// Thùy xinh đẹp yêu cầu phải chỉ rõ multi message với các field lỗi
        /// Tạo list các lỗi. Mỗi lần valid sẽ add lỗi vào
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ChangePassword(ChangePassword dto)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(dto.Password))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.Password), message = "Mật khẩu không được để trống!", });

                if (string.IsNullOrEmpty(dto.NewPassword))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.NewPassword), message = "Mật khẩu không được để trống!", });

                if (string.IsNullOrEmpty(dto.NewPassword2))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.NewPassword2), message = "Mật khẩu không được để trống!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                if (!dto.NewPassword.Equals(dto.NewPassword2))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.NewPassword2), message = "Mật khẩu mới không trùng với nhập lại mật khẩu!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                var entity = await _repo.MstUser.GetAsync(dto.UserId);

                if (entity is null)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không tồn tại!", });

                if (entity is not null && entity.ValidFlg != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(MstUserDto.user_name), message = "Tên đăng nhập không hợp lệ!", });

                if (entity is not null && !entity.password.Equals(StringUtils.Encrypt(dto.Password)))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.Password), message = "Mật khẩu không chính xác!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                entity.upd_dt = DateTime.Now;
                entity.password = StringUtils.Encrypt(dto.NewPassword);
                await _repo.MstUser.UpdateAsync(entity);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Đổi mật khẩu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.Password), message = "Lỗi khi đổi mật khẩu: " + ex.Message });
                return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);
            }
        }

        public async Task<ServiceResult> DeleteUser(int id)
        {
            try
            {
                //Không cho xóa chính mình
                if (_userPrincipalService.UserId == id) return new ServiceResultError("Bạn không thể xóa tài khoản của mình!");
                //Chỉ admin mới có quyền
                if (!_userPrincipalService.Roles.Contains((int)EnumRole.Role.Admin)) return new ServiceResultError("Bạn không có quyền thực hiện thao tác này!");

                var entity = await _repo.MstUser.GetAsync(id);
                if (entity is null) return new ServiceResultError("Nhân viên không tồn tại!");
                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Nhân viên không hợp lệ!");

                entity.ValidFlg = (int)EnumCommon.Status.Invalid;
                entity.upd_dt = DateTime.Now;
                await _repo.MstUser.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Xóa nhân viên thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm user: " + ex.Message);
            }
        }
        #endregion
        #region Other
        public async Task<ServiceResult> Authenticate(Login login)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Username.Trim()))
                    lstErr.Add(new { field = nameof(Login.Username), message = "Username không được để trống!" });

                if (string.IsNullOrEmpty(login.Password) || string.IsNullOrEmpty(login.Password.Trim()))
                    lstErr.Add(new { field = nameof(Login.Password), message = "Password không được để trống!" });

                if (lstErr.Count > 0) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                MstUser user = await _repo.MstUser.FirstOrDefaultAsync(x => x.user_name.ToLower() == login.Username.Trim().ToLower() && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (user is null)
                    lstErr.Add(new { field = nameof(Login.Username), message = "Tên đăng nhập không tồn tại!" });

                if (user is not null && user.enable_fl != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(Login.Username), message = "Tên đăng nhập không hợp lệ!", });

                if (user is not null && user.password != StringUtils.Encrypt(login.Password))
                    lstErr.Add(new { field = nameof(Login.Password), message = "Mật khẩu sai!", });

                if (lstErr.Count > 0) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //To do: Other business
                var claimsToken = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                    new Claim(ClaimTypeConst.USERNAME, user.user_name),
                    new Claim(ClaimTypes.Role, user.auth_fl),
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                    _configuration["Tokens:Audience"],
                    claimsToken,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: creds);
                // create claims
                string strToken = new JwtSecurityTokenHandler().WriteToken(token);

                AuthorizedUser userInfo = new AuthorizedUser
                {
                    UserId = user.user_id,
                    Username = user.user_name,
                    Role = string.IsNullOrEmpty(user.auth_fl) ? new List<int>() : user.auth_fl.Split(",").Select(x => int.Parse(x)).ToList(),
                    Email = user.email,
                    Token = strToken
                };

                return new ServiceResultSuccess("Xác thực thành công!", userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                lstErr.Add(new { field = nameof(Login.Username), message = "Lỗi khi xác thực: " + ex.Message });
                return new ServiceResultError("Lỗi khi xác thực: " + ex.Message, lstErr);
            }

        }
        #endregion
    }
}
