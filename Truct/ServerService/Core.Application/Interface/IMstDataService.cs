using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.SearchConditions;

namespace Core.Application.Interface
{
    public interface IMstDataService
    {
        Task<GenericResponseResult<MstDataDto>> SearchMasterData(MstDataSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> InsertListData(List<MstDataDto> lstData);
        Task<ServiceResult> UpdateListData(List<MstDataDto> lstData);
        Task<ServiceResult> DeleteMasterData(int id);
        Task<ServiceResult> GetMasterDataById(int id);
    }
}
