using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Microsoft.Extensions.Configuration;

namespace Astemo.Infrastructure.Repositories
{
    public class BaseRepositoryWrapperImpl : IBaseRepositoryWrapper
    {
        #region Constructor
        private readonly AstemoContext _repoContext;
        private readonly IConfiguration _configuration;
        public BaseRepositoryWrapperImpl(AstemoContext repositoryContext, IConfiguration configuration)
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

        private IExportListPlanRepository _exportListPlanRepository;
        public IExportListPlanRepository ExportListPlan
        {
            get
            {
                if (_exportListPlanRepository is null)
                    _exportListPlanRepository = new ExportListPlanRepositoryImpl(_repoContext);
                return _exportListPlanRepository;
            }
        }

        private IBoxInfoRepository _boxinfoRepository;
        public IBoxInfoRepository BoxInfo
        {
            get
            {
                if (_boxinfoRepository is null)
                    _boxinfoRepository = new BoxInfoRepositoryImpl(_repoContext);
                return _boxinfoRepository;
            }
        }

        private IExportHistoryListRepository _exportHistoryListRepository;
        public IExportHistoryListRepository ExportHistoryList
        {
            get
            {
                if (_exportHistoryListRepository is null)
                    _exportHistoryListRepository = new ExportHistoryListRepositoryImpl(_repoContext);
                return _exportHistoryListRepository;
            }
        }
        
        private IMstDataRepository _mstDataRepository;
        public IMstDataRepository MstData
        {
            get
            {
                if (_mstDataRepository is null)
                    _mstDataRepository = new MstDataRepositoryImpl(_repoContext);
                return _mstDataRepository;
            }
        }

        private IEcuDataRepository _ecuDataRepository;
        public IEcuDataRepository EcuData
        {
            get
            {
                if (_ecuDataRepository is null)
                    _ecuDataRepository = new EcuDataRepositoryImpl(_repoContext);
                return _ecuDataRepository;
            }
        }

        private IEcuExportedRepository _ecuExportedRepository;
        public IEcuExportedRepository EcuExported
        {
            get
            {
                if (_ecuExportedRepository is null)
                    _ecuExportedRepository = new EcuExportedRepositoryImpl(_repoContext);
                return _ecuExportedRepository;
            }
        }

        #endregion

        public async Task SaveAync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
