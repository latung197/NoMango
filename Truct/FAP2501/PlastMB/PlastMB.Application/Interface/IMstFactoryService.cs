
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.CustomModels;

namespace PlastMB.Application.Interface
{
    public interface IMstFactoryService
    {
        Task<GenericResponseResult<MstFactoryDto>> SearchMstFactory(MstFactorySearchImpl condition, bool blnExport = false);
        //Task<ServiceResult> UpdateEcuData(EcuDataDto dto);
        //Task<ServiceResult> DeleteEcuData(int id);
        //Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data);
    }
}
