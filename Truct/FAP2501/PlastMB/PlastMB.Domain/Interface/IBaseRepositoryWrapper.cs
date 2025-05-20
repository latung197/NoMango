
namespace PlastMB.Domain.Interface
{
    public interface IBaseRepositoryWrapper
    {
        #region Properties
        IMstUserRepository MstUser { get; }
        ITrnImportHistoryRepository TrnImportHistory { get; }
        ITrnImportHistoryDetailRepository TrnImportHistoryDetail { get; }
        IMstMachineRepository MstMachine { get; }
        IMstFactoryRepository MstFactory { get; }
        ITrnOperationOeeRepository TrnOperationOee { get; }
        ITrnOperationResultRepository TrnOperationResult { get; }
        #endregion
        Task SaveAync();
    }
}
