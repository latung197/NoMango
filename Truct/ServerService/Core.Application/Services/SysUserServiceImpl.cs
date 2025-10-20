using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Infrastructure.Constants;
using Core.Infrastructure.ContextAccessors;
using Core.Utils;
using Core.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Domain.Entity.SystemEntities;

namespace Core.Application.Services
{
    public class SysUserServiceImpl : ISysUserService
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
        public SysUserServiceImpl(IBaseRepositoryWrapper repo
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
        public async Task<GenericResponseResult<AuthorizedUser>> SearchUser(SysUserSearchImpl condition, bool blnExport = false)
        {
           
            var iqResult = from u in _repo.SysUser.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.Username) || u.UserName.ToLower().Contains(condition.Username.ToLower()))
                           && (string.IsNullOrEmpty(condition.Email) || u.Email.ToLower().Contains(condition.Email.ToLower()))
                           && (string.IsNullOrEmpty(condition.Fullname) || u.FullName.ToLower().Contains(condition.Fullname.ToLower()))
                           && (condition.Enable == u.EnableFl)
                           && (condition.Role == -1 || u.AuthFl.Contains(condition.Role.ToString()))
                           && u.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby u.UserId descending, (u.UpdateTime ?? u.CreateTime) descending
                           select _mapper.Map<SysUserDto>(u);

            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<AuthorizedUser>();
            }

            var lstData = await iqResult.ToListAsync();
            var lstResult = new List<AuthorizedUser>();
            foreach (var item in lstData){
                var author = new AuthorizedUser();
                author.UserId = item.UserId;
                author.Username = item.UserName;
                author.Fullname = item.FullName;
                author.Employeecode = item.EmployeeCode;
                author.Email = item.Email;
                author.Token = string.Empty;
                author.Role = string.IsNullOrEmpty(item.AuthFl) ? new List<int>() : item.AuthFl.Split(",").Select(x => int.Parse(x)).ToList();
                lstResult.Add(author);
            }
            return new GenericResponseResult<AuthorizedUser>(lstResult);
        }
        public async Task<ServiceResult> GetUserById(int id)
        {
            //Chỉ admin mới có quyền
            //if (!_userPrincipalService.Roles.Contains((int)EnumRole.Role.Admin)) return new ServiceResultError("Bạn không có quyền thực hiện thao tác này!");

            var entity = await _repo.SysUser.GetAsync(id);
            if (entity is null) return new ServiceResultError("Nhân viên không tồn tại");
            if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Nhân viên không hợp lệ!");
            var dto = _mapper.Map<SysUserDto>(entity);

            //if (string.IsNullOrEmpty(dto.auth_fl))
            //    dto.UserRoles = new List<int>();
            //else
            //    dto.Role = dto.auth_fl.Split(",").Select(x => int.Parse(x)).ToList();
            ////Admin need original password
            //dto.password = StringUtils.Decrypt(entity.password);
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
        public async Task<ServiceResult> InsertUser(SysUserDto dto)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.UserName.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không được để trống!", });

                if (string.IsNullOrEmpty(dto.FullName) || string.IsNullOrEmpty(dto.FullName.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên nhân viên không được để trống!", });

                if (string.IsNullOrEmpty(dto.EmployeeCode) || string.IsNullOrEmpty(dto.EmployeeCode.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.EmployeeCode), message = "Mã nhân viên không được để trống!", });

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    if (!StringUtils.IsValidEmail(dto.Email))
                        lstErr.Add(new { field = nameof(SysUserDto.Email), message = "Email không đúng định dạng!", });

                    var emailExist = await _repo.SysUser.AnyAsync(x => x.Email.ToLower().Equals(dto.Email.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (emailExist)
                        lstErr.Add(new { field = nameof(SysUserDto.Email), message = "Email đã tồn tại!", });
                }

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                var userExist = await _repo.SysUser.AnyAsync(x => x.UserName.ToLower().Equals(dto.UserName.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (userExist)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập đã tồn tại!", });

                var employeeCodeExist = await _repo.SysUser.AnyAsync(x => x.EmployeeCode.ToLower().Equals(dto.EmployeeCode.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (employeeCodeExist)
                    lstErr.Add(new { field = nameof(SysUserDto.EmployeeCode), message = "Mã nhân viên đã tồn tại!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                var entity = _mapper.Map<SysUser>(dto);
                entity.UpdateTime = DateTime.Now;

                string pass = _configuration["DefaultPassword"];

                if (!string.IsNullOrEmpty(dto.PasswordHash))
                    pass = dto.PasswordHash;

                entity.PasswordHash = StringUtils.Encrypt(pass);
                entity.ValidFlg = (int)EnumCommon.Status.Valid;
                //if (dto.Role is null || dto.Role.Count == 0)
                //    entity.auth_fl = string.Empty;
                //else
                //    entity.auth_fl = string.Join(",", dto.Role.Select(x => x.ToString()));

                await _repo.SysUser.InsertAsync(entity);
                await _repo.SaveAync();

                var returnData = new Login { Username = dto.UserName, Password = pass };
                return new ServiceResultSuccess($"Thêm nhân viên thành công!", returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                //lstErr.Add(new { field = nameof(MstUserDto.UserName), message = "Lỗi khi thêm user: " + ex.Message });
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
        public async Task<ServiceResult> UpdateUser(SysUserDto dto)
        {
            var lstErr = new List<object>();
            try
            {
                if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.UserName.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không được để trống!", });

                if (string.IsNullOrEmpty(dto.FullName) || string.IsNullOrEmpty(dto.FullName.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên nhân viên không được để trống!", });

                if (string.IsNullOrEmpty(dto.EmployeeCode) || string.IsNullOrEmpty(dto.EmployeeCode.Trim()))
                    lstErr.Add(new { field = nameof(SysUserDto.EmployeeCode), message = "Mã nhân viên không được để trống!", });

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    if (!StringUtils.IsValidEmail(dto.Email))
                        lstErr.Add(new { field = nameof(SysUserDto.Email), message = "Email không đúng định dạng!", });

                    if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                    var emailExist = await _repo.SysUser.AnyAsync(x => x.Email.ToLower().Equals(dto.Email.ToLower()) && x.UserId != dto.UserId && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (emailExist)
                        lstErr.Add(new { field = nameof(SysUserDto.Email), message = "Email đã tồn tại!", });
                }

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //Check tồn tại bản ghi trong DB
                var entity = await _repo.SysUser.GetAsync(dto.UserId);
                if (entity is null)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không tồn tại" });

                if (entity is not null && entity.ValidFlg != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không hợp lệ" });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //Check trùng username trong database
                var existUser = await _repo.SysUser.AnyAsync(x => x.UserName.ToLower().Equals(dto.UserName.ToLower()) && x.UserId != dto.UserId && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (existUser)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập đã tồn tại!", });
                
                //Check trùng mã nhân viên trong database
                var employeeCodeExist = await _repo.SysUser.AnyAsync(x => x.EmployeeCode.ToLower().Equals(dto.EmployeeCode.ToLower()) && x.UserId != dto.UserId && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (employeeCodeExist)
                    lstErr.Add(new { field = nameof(SysUserDto.EmployeeCode), message = "Mã nhân viên đã tồn tại!", });


                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                string pass = _configuration["DefaultPassword"];

                if (!string.IsNullOrEmpty(dto.PasswordHash))
                    pass = dto.PasswordHash;

                entity.UserName = dto.UserName;
                entity.EnableFl = dto.EnableFl;
                entity.Email = dto.Email;
                entity.AuthFl = dto.AuthFl;
                entity.FullName = dto.FullName;
                entity.EmployeeCode = dto.EmployeeCode;
                entity.GenDer = dto.GenDer;
                entity.UpdateTime = DateTime.Now;

                //if (dto.Role is null || dto.Role.Count == 0)
                //    entity.AuthFl = string.Empty;
                //else
                //    entity.AuthFl = string.Join(",", dto.Role.Select(x => x.ToString()));
                entity.PasswordHash = StringUtils.Encrypt(pass);
                entity.ValidFlg = (int)EnumCommon.Status.Valid;
                await _repo.SysUser.UpdateAsync(entity);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Cập nhật nhân viên thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Lỗi khi cập nhật user: " + ex.Message });
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

                var entity = await _repo.SysUser.GetAsync(dto.UserId);

                if (entity is null)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không tồn tại!", });

                if (entity is not null && entity.ValidFlg != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(SysUserDto.UserName), message = "Tên đăng nhập không hợp lệ!", });

                if (entity is not null && !entity.PasswordHash.Equals(StringUtils.Encrypt(dto.Password)))
                    lstErr.Add(new { field = nameof(CustomModels.Others.ChangePassword.Password), message = "Mật khẩu không chính xác!", });

                if (lstErr.Any()) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                entity.UpdateTime = DateTime.Now;
                entity.PasswordHash = StringUtils.Encrypt(dto.NewPassword);
                await _repo.SysUser.UpdateAsync(entity);
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

                var entity = await _repo.SysUser.GetAsync(id);
                if (entity is null) return new ServiceResultError("Nhân viên không tồn tại!");
                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Nhân viên không hợp lệ!");

                entity.ValidFlg = (int)EnumCommon.Status.Invalid;
                entity.UpdateTime = DateTime.Now;
                await _repo.SysUser.UpdateAsync(entity);
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

                SysUser user = await _repo.SysUser.FirstOrDefaultAsync(x => x.UserName.ToLower() == login.Username.Trim().ToLower() && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (user is null)
                    lstErr.Add(new { field = nameof(Login.Username), message = "Tên đăng nhập không tồn tại!" });

                if (user is not null && user.EnableFl != (int)EnumCommon.Status.Valid)
                    lstErr.Add(new { field = nameof(Login.Username), message = "Tên đăng nhập không hợp lệ!", });

                if (user is not null && user.PasswordHash != StringUtils.Encrypt(login.Password))
                    lstErr.Add(new { field = nameof(Login.Password), message = "Mật khẩu sai!", });

                if (lstErr.Count > 0) return new ServiceResultError("Đã có lỗi xảy ra!", lstErr);

                //To do: Other business
                var claimsToken = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypeConst.USERNAME, user.UserName),
                    new Claim(ClaimTypes.Role, user.AuthFl),
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
                    UserId = user.UserId,
                    Username = user.UserName,
                    Role = string.IsNullOrEmpty(user.AuthFl) ? new List<int>() : user.AuthFl.Split(",").Select(x => int.Parse(x)).ToList(),
                    Email = user.Email,
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
