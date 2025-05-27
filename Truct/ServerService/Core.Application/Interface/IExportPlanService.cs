using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;

namespace Core.Application.Interface
{
    public interface IExportPlanService
    {
        Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlan(ExportPlanSearchImpl condition, bool blnExport = false);
        Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlans(ExportPlanSearchImpl condition, bool blnExport = false);
        Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlanPre(ExportPlanPreSearchImpl condition, bool blnExport = false);
        Task<GenericResponseResult<ExportHistoryPlanListDto>> SearchExportHistoryPlan(ExportPlanSearchImpl condition, bool blnExport = true);
        Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanById(int PlanID);
        Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanByIdAndSerial(int PlanID, string boxSerial);
        Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanBySerial(string boxSerial);
        Task<GenericResponseResult<PrepareProduct>> SearchPrepareProduct(PrepareProductSearchImpl condition, bool blnExport = false);
        Task<ServiceResult> UpdateListExportHistory(List<ExportHistoryListDto> lstData);
        Task<ServiceResult> DeleteExportPlan(int ExportPlanID);
        Task<ServiceResult> DeleteBoxInfo(int BoxID);
        Task<ServiceResult> GetExportPlanById(int IDPlan);
        Task<ServiceResult> UpdateExportPlan(ExportListPlanDto data);
        Task<ServiceResult> InsertExportPlan(ExportListPlanDto data);
        Task<ServiceResult> InsertListExportPlan(List<ExportListPlanImportDto> lstData);
        Task<ServiceResult> InsertListWarningPlan(ImportPlan condition);
        Task<ServiceResult> GetListExportPlanPre();
        Task<GenericResponseResult<BoxStampInfo>> GetExportPlanStampInfo(int id);
        Task<GenericResponseResult<BoxStampInfo>> GetExportPlansStampInfo(int id);
        Task<ServiceResult> GetSpecialStampInfo(SpecialStampInfo info);
        Task<ServiceResult> CheckBoxInfo(string boxSerial);
        Task<ServiceResult> UpdateListBillState(List<UpdateStatus> lstData);
        Task<ServiceResult> UpdatePrepareProductOrderState(List<UpdateStatus> lstData);
        Task<ServiceResult> UpdatePrepareBoxInfoOrderState(List<UpdateStatus> lstData);
        Task<ServiceResult> InsertListExportHistory(List<ImportPlanHistory> lstData);
        Task<ServiceResult> InsertScanBoxResult(List<ExportPlanScanBoxResult> lstData);
        Task<ServiceResult> InsertScanBoxV2Result(List<ExportPlanScanBoxResult> lstData);
        Task<ServiceResult> AddScanBoxResult(List<ExportPlanScanBoxResult> lstData);
        Task<ServiceResult> AddScanBoxV2Result(List<ExportPlanScanBoxResult> lstData);
        Task<ServiceResult> InsertHistoryPlan(ExportHistoryListDto dto);
        Task<ServiceResult> UpdatetHistoryPlan(ExportHistoryListDto dto);
    }
}
