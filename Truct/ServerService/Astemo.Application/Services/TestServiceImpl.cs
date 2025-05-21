using Astemo.Application.Interface;
using Astemo.Domain.Interface;
using Astemo.Utils.LogUtils;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Astemo.Application.Services
{
    public class TestServiceImpl : ITestService
    {
        #region Properties
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        #endregion
        #region Constructor
        public TestServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public Dictionary<int, string> TestDict()
        {
            var dict = new Dictionary<int, string>();
            for (int i = 0; i < 10; i++)
            {
                dict.Add(i, i.ToString());
            }

            return dict;
        }
    }
}
