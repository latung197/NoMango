using Worker.Application.Enum;

namespace Worker.Application.BaseHttp.Interface
{
    public interface IBaseHttpClient
    {
        Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null, Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new();
        Task<string> GetAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PostAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
           where T : new();
        Task<bool> PostAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> PutAsync<T>(string domain, string apiEndpoint,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<bool> PutAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
        Task<T> DeleteAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new();
        Task<bool> DeleteAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer);
    }
}
