using Worker.Application.BaseHttp.Interface;
using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.Interface;
using PlastMB.Utils.LogUtils;
using Microsoft.Extensions.Configuration;
using Worker.Application.CustomModels.SearchConditions;
using System.Net.Http.Json;
using System.Text;
using System.Net.Http;

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
        private readonly HttpClient _httpClient;

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
            _httpClient = new HttpClient();
        }


        #endregion
        #region Search

        public async Task<string> SearchMstMachine(string jsonContent)
        {
            try
            {
                var client = _clientFatory.Create();
                var apiUrl = $"api/MstMachine/search-MstMachine";
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                apiUrl = _apiDomain + "/" + apiUrl;
                var response = await _httpClient.PostAsync(apiUrl, content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while post data !" + ex.Message);
                return "";
            }
        }

        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListTrnImportHistory(List<TrnImportHistoryDto> data)
        {
            try
            {
                var client = _clientFatory.Create();
                var apiUrl = $"api/TrnImportHistory/import-list-History";
                var response = await client.PostAsync<ServiceResult>(_apiDomain, apiUrl, data);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while post data !" + ex.Message);
                return new ServiceResultError("Error while post data TrnImportHistory!" + ex.Message);
            }
        }

        public async Task<ServiceResult> ImportListTrnOperationOee(List<TrnOperationOeeDto> data)
        {
            try
            {
                var client = _clientFatory.Create();
                var apiUrl = $"api/TrnOperationOee/import-list-TrnOperationOee";
                var response = await client.PostAsync<ServiceResult>(_apiDomain, apiUrl, data);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while post data !" + ex.Message);
                return new ServiceResultError("Error while post data TrnOperationOee!" + ex.Message);
            }
        }

        public async Task<ServiceResult> ImportListTrnOperationResult(List<TrnOperationResultDto> data)
        {
            try
            {
                var client = _clientFatory.Create();
                var apiUrl = $"api/TrnOperationResult/import-list-TrnOperationResult";
                var response = await client.PostAsync<ServiceResult>(_apiDomain, apiUrl, data);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while post data!" + ex.Message);
                return new ServiceResultError("Error while post data TrnOperationResult!" + ex.Message);
            }
        }

        #endregion
    }
}
