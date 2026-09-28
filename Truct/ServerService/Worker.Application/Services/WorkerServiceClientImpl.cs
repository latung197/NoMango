using Worker.Application.BaseHttp.Interface;
using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.Interface;
using Core.Utils.LogUtils;
using Microsoft.Extensions.Configuration;
using Worker.Application.Constants;
using Newtonsoft.Json.Linq;

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
                var username = _configuration["WorkerUsername"];
                var password = _configuration["WorkerPassword"];
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return new ServiceResultError("Worker credentials are not configured.");

                var login = await client.PostAsync<ServiceResult>(_apiDomain, "api/sysuser/login",
                    new { Username = username, Password = password });
                var token = login?.Code == CommonConstant.SUCCESS
                    ? (login.Data as JObject)?["token"]?.Value<string>()
                    : null;
                if (string.IsNullOrWhiteSpace(token))
                    return new ServiceResultError("Worker login failed.");

                var apiUrl = $"api/ecudata/import-list-ecu-data";
                var response = await client.PostAsync<ServiceResult>(_apiDomain, apiUrl, data,
                    accessToken: token);
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
