using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.Dtos;
using Astemo.Application.CustomModels.SearchConditions;

namespace Astemo.Application.Interface
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
