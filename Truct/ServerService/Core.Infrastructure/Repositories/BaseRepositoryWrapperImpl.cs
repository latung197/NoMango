using Core.Domain.Interface;
using Core.Domain.Interface.SystemInterface;
using Core.Infrastructure.Context;
using Core.Infrastructure.Repositories.SysRepositories;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure.Repositories
{
    public class BaseRepositoryWrapperImpl : IBaseRepositoryWrapper
    {
        #region Constructor
        private readonly CoreContext _repoContext;
        private readonly IConfiguration _configuration;
        public BaseRepositoryWrapperImpl(CoreContext repositoryContext, IConfiguration configuration)
        {
            _repoContext = repositoryContext;
            _configuration = configuration;
        }
        #endregion

        #region Properties

        private ISysUserRepository _mstUserRepository;
        public ISysUserRepository SysUser
        {
            get
            {
                if (_mstUserRepository is null)
                    _mstUserRepository = new SysUserRepositoryImpl(_repoContext);
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
