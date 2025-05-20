using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.Others;
using PlastMB.Domain.Entity;
using AutoMapper;

namespace PlastMB.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MstUser, MstUserDto>();
            CreateMap<MstUserDto, MstUser>();
            CreateMap<TrnImportHistoryDto, TrnImportHistory>();
            CreateMap<TrnImportHistory, TrnImportHistoryDto>();
            CreateMap<MstMachine, MstMachineDto>();
            CreateMap<MstFactory, MstFactoryDto>();
            CreateMap<MstFactoryDto, MstFactory>();
            CreateMap<TrnOperationOee, TrnOperationOeeDto>();
            CreateMap<TrnOperationOeeDto, TrnOperationOee>();
            CreateMap<TrnOperationResult, TrnOperationResultDto>();
            CreateMap<TrnOperationResultDto,TrnOperationResult >();
            CreateMap<TrnImportHistoryDetail, TrnImportHistoryDetailDto>();
            CreateMap<TrnImportHistoryDetailDto,TrnImportHistoryDetail>();

        }
    }
}
