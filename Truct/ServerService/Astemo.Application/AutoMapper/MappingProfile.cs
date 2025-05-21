using Astemo.Application.CustomModels.Dtos;
using Astemo.Application.CustomModels.Others;
using Astemo.Domain.Entity;
using AutoMapper;

namespace Astemo.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MstUser, MstUserDto>();
            CreateMap<MstUserDto, MstUser>();
            CreateMap<ExportListPlan, ExportListPlanDto>();
            CreateMap<ExportListPlan, ExportListPlanImportDto>();
            CreateMap<ExportListPlanDto, ExportListPlan>();
            CreateMap<ExportListPlanImportDto, ExportListPlan>();
            CreateMap<ExportHistoryListDto, ExportHistoryList>();
            CreateMap<ExportHistoryList, ExportHistoryListDto>();
            CreateMap<MstData, MstDataDto>();
            CreateMap<MstDataDto, MstData>();
            CreateMap<EcuDataDto, EcuExported>();
            CreateMap<EcuDataDto, EcuData>();
            CreateMap<EcuData, EcuDataDto>();
            CreateMap<EcuExported, EcuDataDto>();
        }
    }
}
