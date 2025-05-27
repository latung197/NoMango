using Core.Application.CustomModels;
using Core.Application.CustomModels.Others;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Utils.LogUtils;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Services
{
    public class HandyServiceImpl : IHandyService
    {
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        public HandyServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        #region Search
        #endregion
        #region CRUD
        #region Insert
        #endregion
        #region Update
        #endregion
        #region Delete
        #endregion
        #endregion
        #region Others
        /// <summary>
        /// Check thông tin máy Handy gửi về NG hay OK
        /// </summary>
        /// <param name="data">Dữ liệu máy handy gửi về</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <remarks>3.Khi PC nhận được thông tin chuyền về của handy thực hiện các action bên dưới</remarks>
        /// <remarks>3.1 Nếu trường dùng để tham chiếu là[Mã sản phẩm]</remarks>
        /// <remarks>Bước 1: Dùng mã sản phẩm mà handy gủi về vào trong trong bảng master sau đó lấy giá trị tương ứng ở cột : Dấu nhận dạng</remarks>
        /// <remarks>Bước 2: Dùng mã Hu gủi về từ máy handy sau đó vào bảng Line#3 hoặc Line#4</remarks>
        /// <remarks>tương ứng tương ứng với mã Hu lấy giá trị ở cột inlaze(lấy 6 trữ cái đầu tiên)</remarks>
        /// <remarks>Bước 3: So sánh giá trị của[Dấu nhận dạng] ở bước 1 với giá trị của 6 trữ cái đầu của cột inlaze xem có trùng khớp không </remarks>
        /// <remarks>=>Nếu không trùng trả về NG cho bên phía handy</remarks>
        /// <remarks>=> Nếu trùng check thêm cột result trong dữ liệu gia công(Line#3 hoặc Line#4)</remarks>
        /// <remarks>Nếu trường dữ liệu gia công (ECU) cột result là ok thì trà ok về cho handy, nếu là Ng thì trả NG về cho handy</remarks>
        /// <remarks>Update 18/01/2024: Một số dữ liệu Dấu nhận dạng chỉ có 5 chữ cái nên cần so 5 chữ cái đầu với chữ khắc Laser</remarks>
        public async Task<ServiceResult> CheckHandyInfo(HandyInfo data)
        {
            try
            {
                MstData mstData;

                if (data.Type == (int)EnumMasterType.Type.ProductCode)
                    mstData = await _repo.MstData.FirstOrDefaultAsync(x => x.ProductCode == data.ProductCode && x.Customer.ToLower().Equals(data.CustomerCode.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);
                else if (data.Type == (int)EnumReferenceType.Type.InternalCode)
                    mstData = await _repo.MstData.FirstOrDefaultAsync(x => x.InternalDrawingCode == data.ProductCode && x.Customer.ToLower().Equals(data.CustomerCode.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);
                else
                    mstData = await _repo.MstData.FirstOrDefaultAsync(x => x.AssemblyProductCode == data.ProductCode && x.Customer.ToLower().Equals(data.CustomerCode.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (mstData is null) return new ServiceResultNG("Dữ liệu master không tồn tại!");

                // Chỉ tìm kết quả gia công Ok
                var ecu = await _repo.EcuData.FirstOrDefaultAsync(x => x.HUCode == data.HUCode && x.Result == (int)EnumCommon.Status.Valid && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (ecu is null) return new ServiceResultNG("ERROR_HU_NOT_EXITS"); //Ecu data không tồn tại!

                if (ecu.PackState > (int)EnumPackState.State.NotPacked) return new ServiceResultNG("PACKED");   //Sản phẩm đã được nhập kho!

                bool isMatch = false;

                //Dấu nhận dạng chỉ có 5 ký tự. nếu khác thì lấy tối đa 6 kí tự
                if (mstData!.IdentityMark == null || ecu!.LaserPrinting == null)
                    isMatch = false;
                else if (ecu.LaserPrinting.Length < mstData.IdentityMark.Length)
                    isMatch = false;
                else if (mstData!.IdentityMark.Length == 5)
                    isMatch = mstData.IdentityMark.Equals(ecu!.LaserPrinting.Substring(0, 5));
                else if (mstData!.IdentityMark.Length >= 6)
                    isMatch = mstData.IdentityMark.Substring(0, 6).Equals(ecu!.LaserPrinting.Substring(0, 6));
                else
                    isMatch = mstData.IdentityMark.Equals(ecu.LaserPrinting.Substring(0, mstData.IdentityMark.Length));

                if (!isMatch) return new ServiceResultNG($"Dau nhan dang khong dung: {mstData.IdentityMark} <> {ecu.LaserPrinting}"); //$"Dấu nhận dạng không đúng: {mstData.IdentityMark} <> {ecu.LaserPrinting}"

                //if (ecu.Result != (int)EnumCommon.Status.Valid) return new ServiceResultNG("Kết quả gia công NG!");

                return new ServiceResultSuccess("Kết quả gia công Ok!", new { Laser = ecu.LaserPrinting });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi kiểm tra dữ liệu handy: " + ex.Message);
            }
        }
        #endregion
    }
}
