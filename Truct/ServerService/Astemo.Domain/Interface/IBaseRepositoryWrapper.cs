
namespace Core.Domain.Interface
{
    public interface IBaseRepositoryWrapper
    {
        #region Properties
        IMstUserRepository MstUser { get; }
        IExportListPlanRepository ExportListPlan { get; }
        IExportHistoryListRepository ExportHistoryList { get; }
        IMstDataRepository MstData { get; }
        IEcuDataRepository EcuData { get; }
        IEcuExportedRepository EcuExported { get; }
        IBoxInfoRepository BoxInfo { get; }
        #endregion
        Task SaveAync();
    }
}
