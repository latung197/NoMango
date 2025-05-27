using Worker.Application.BaseHttp.Interface;
using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.Interface;
using Core.Utils.LogUtils;
using Microsoft.Extensions.Configuration;

namespace Worker.Application.Services
{
    public class WorkerServiceClientImpl : IWorkerServiceClient
    {
        #region Properties
        private readonly IBaseHttpClientFactory _clientFatory;
        private readonly string _apiDomain;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Log
        private readonly ILoggerManager _logger;
        #endregion
        #region Constructor
        public WorkerServiceClientImpl(IBaseHttpClientFactory factory
            , IConfiguration configuration
            , ILoggerManager logger)
        {
            _clientFatory = factory;
            _configuration = configuration;
            _logger = logger;
            _apiDomain = configuration["ApiDomain"];
        }
        #endregion
        #region Search

        #endregion
        #region CRUD
        public async Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data)
        {
            try
            {
                var client = _clientFatory.Create();
                var apiUrl = $"api/ecudata/import-list-ecu-data";
                var response = await client.PostAsync<ServiceResult>(_apiDomain, apiUrl, data);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while post data Ecu!" + ex.Message);
                return new ServiceResultError("Error while post data Ecu!" + ex.Message);
            }
        }
        #endregion
    }
}
