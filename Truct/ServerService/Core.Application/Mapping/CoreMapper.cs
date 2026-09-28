using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Domain.Entity;
using Core.Domain.Entity.SystemEntities;
using Riok.Mapperly.Abstractions;

namespace Core.Application.Mapping;

// DTOs intentionally expose only part of their entity's fields.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CoreMapper
{
    public partial SysUserDto ToSysUserDto(SysUser source);
    public partial SysUser ToSysUser(SysUserDto source);
    public partial ExportListPlanDto ToExportListPlanDto(ExportListPlan source);
    public partial ExportListPlanImportDto ToExportListPlanImportDto(ExportListPlan source);
    public partial ExportListPlan ToExportListPlan(ExportListPlanDto source);
    public partial ExportListPlan ToExportListPlan(ExportListPlanImportDto source);
    public partial void UpdateExportListPlan(ExportListPlanDto source, ExportListPlan target);
    public partial void UpdateExportListPlan(ExportListPlanImportDto source, ExportListPlan target);
    public partial ExportHistoryList ToExportHistoryList(ExportHistoryListDto source);
    public partial ExportHistoryListDto ToExportHistoryListDto(ExportHistoryList source);
    public partial void UpdateExportHistoryList(ExportHistoryListDto source, ExportHistoryList target);
    public partial MstDataDto ToMstDataDto(MstData source);
    public partial MstData ToMstData(MstDataDto source);
    public partial void UpdateMstData(MstDataDto source, MstData target);
    public partial EcuExported ToEcuExported(EcuDataDto source);
    public partial EcuData ToEcuData(EcuDataDto source);
    public partial EcuDataDto ToEcuDataDto(EcuData source);
    public partial EcuDataDto ToEcuDataDto(EcuExported source);
}
