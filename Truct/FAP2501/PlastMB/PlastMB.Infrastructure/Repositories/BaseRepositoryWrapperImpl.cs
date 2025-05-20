using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using Microsoft.Extensions.Configuration;

namespace PlastMB.Infrastructure.Repositories
{
    public class BaseRepositoryWrapperImpl : IBaseRepositoryWrapper
    {
        #region Constructor
        private readonly PlastMBContext _repoContext;
        private readonly IConfiguration _configuration;
        public BaseRepositoryWrapperImpl(PlastMBContext repositoryContext, IConfiguration configuration)
        {
            _repoContext = repositoryContext;
            _configuration = configuration;
        }
        #endregion

        #region Properties

        private IMstUserRepository _mstUserRepository;
        public IMstUserRepository MstUser
        {
            get
            {
                if (_mstUserRepository is null)
                    _mstUserRepository = new MstUserRepositoryImpl(_repoContext);
                return _mstUserRepository;
            }
        }


        private ITrnImportHistoryRepository _importHistoryRepository;
        public ITrnImportHistoryRepository TrnImportHistory
        {
            get
            {
                if (_importHistoryRepository is null)
                    _importHistoryRepository = new TrnImportHistoryRepositoryImpl(_repoContext);
                return _importHistoryRepository;
            }
        }

        private IMstMachineRepository _mstMachineRepository;

        public IMstMachineRepository MstMachine
        {
            get
            {
                if (_mstMachineRepository is null)
                    _mstMachineRepository = new MstMachineRepositoryImpl(_repoContext);
                return _mstMachineRepository;
            }
        }

        IMstFactoryRepository _mstFactoryRepository;
        public IMstFactoryRepository MstFactory
        {
            get
            {
                if (_mstFactoryRepository is null)
                    _mstFactoryRepository = new MstFactoryRepositoryImpl(_repoContext);
                return _mstFactoryRepository;
            }
        }

        ITrnOperationOeeRepository _trnOperationOeeRepository;
        public ITrnOperationOeeRepository TrnOperationOee
        {
            get
            {
                if (_trnOperationOeeRepository is null)
                    _trnOperationOeeRepository = new TrnOperationOeeRepositoryImpl(_repoContext);
                return _trnOperationOeeRepository;
            }
        }

        ITrnOperationResultRepository _trnOperationResultRepository;
        public ITrnOperationResultRepository TrnOperationResult
        {
            get
            {
                if(_trnOperationResultRepository is null)
                    _trnOperationResultRepository = new TrnOperationResultRepositoryImpl(_repoContext); 
                return _trnOperationResultRepository;
            }

        }

        ITrnImportHistoryDetailRepository _trnImportHistoryDetailRepository;

        public ITrnImportHistoryDetailRepository TrnImportHistoryDetail
        {
            get
            {
                if (_trnImportHistoryDetailRepository is null)
                    _trnImportHistoryDetailRepository = new TrnImportHistoryDetailRepositoryImpl(_repoContext);
                return _trnImportHistoryDetailRepository;
            }

        }


        #endregion

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
