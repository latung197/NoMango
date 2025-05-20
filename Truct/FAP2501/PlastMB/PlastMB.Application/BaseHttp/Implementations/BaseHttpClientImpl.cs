
using PlastMB.Application.BaseHttp.Interface;
using PlastMB.Application.Enum;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace PlastMB.Application.BaseHttp.Implementations
{
    public class BaseHttpClientImpl : IBaseHttpClient
    {
        private readonly HttpClient _httpClient;
        public BaseHttpClientImpl(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        #region Get
        public async Task<T> GetAsync<T>(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null, Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer) where T : new()
        {

            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await ProcessReponseAsync<T>(response);
            }
        }

        public async Task<string> GetAsync(string domain, string apiEndpoint, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion Get

        #region Post

        public async Task<T> PostAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
           Dictionary<string, string> headers = null,
           string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
           where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dataStr = string.Empty;
            if (data != null)
            {
                if (!(data is string))
                {
                    dataStr = JsonConvert.SerializeObject(data);
                }
                else
                {
                    dataStr = data.ToString();
                }
            }
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            AppendHttpContent(message, dataStr);


            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on post async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> PostAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri); //Thêm param
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dataStr = string.Empty;
            if (data != null)
            {
                if (!(data is string))
                {
                    dataStr = JsonConvert.SerializeObject(data);
                }
                else
                {
                    dataStr = data.ToString();
                }
            }
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);
            AppendHttpContent(message, dataStr);
            try
            {
                var response = await _httpClient.SendAsync(message).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion Post

        #region Put
        public async Task<T> PutAsync<T>(string domain, string apiEndpoint,
            object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var dataStr = string.Empty;
            if (data != null)
            {
                dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
            AppendHttpContent(message, dataStr);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on put async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> PutAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            var dataStr = string.Empty;
            if (data != null)
            {
                dataStr = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

            AppendHttpContent(message, dataStr);
            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region Delete
        public async Task<T> DeleteAsync<T>(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
            where T : new()
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await ProcessReponseAsync<T>(response);
                }
                else
                {
                    throw new HttpRequestException($"{nameof(_httpClient)} :error on Delete async {JsonConvert.SerializeObject(response)}"); // request không thành công
                }
            }
        }

        public async Task<bool> DeleteAsync(string domain, string apiEndpoint, object data = null, Dictionary<string, string> requestParams = null,
            Dictionary<string, string> headers = null,
            string accessToken = "", AccessTokenType accessTokenType = AccessTokenType.Bearer)
        {
            var requestUri = $"{domain}";
            if (!string.IsNullOrWhiteSpace(apiEndpoint))
            {
                requestUri += $"/{apiEndpoint}";
            }
            requestUri = PopulateRequestParam(requestParams, requestUri);
            var message = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AppendAccessToken(message, accessToken, accessTokenType);
            AddRequestHeader(message, headers);

            using (var response = await _httpClient.SendAsync(message).ConfigureAwait(false))
            {
                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Thêm param vào URI
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private string PopulateRequestParam(Dictionary<string, string> requestParams, string requestUri)
        {
            if (requestParams != null && requestParams.Count > 0)
            {
                requestUri += "?";

                foreach (var item in requestParams)
                {
                    requestUri += $"{item.Key}={item.Value}&";
                }

                if (requestUri.EndsWith("&"))
                {
                    requestUri = requestUri.Substring(0, requestUri.Length - 1);
                }
            }

            return requestUri;
        }

        private void AddRequestHeader(HttpRequestMessage requestMessage, Dictionary<string, string> headers)
        {
            //client.Timeout = TimeSpan.FromSeconds(GetRequestTimeout());
            if (requestMessage != null && headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }
        }
        private void AppendHttpContent(HttpRequestMessage requestMessage, string httpContentStr)
        {
            //client.Timeout = TimeSpan.FromSeconds(GetRequestTimeout());
            if (requestMessage != null && !string.IsNullOrWhiteSpace(httpContentStr))
            {
                requestMessage.Content = new StringContent(httpContentStr, Encoding.UTF8, "application/json");
            }
        }

        /// <summary>
        /// Append Access Token vào HttpRequest
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="accessToken"></param>
        /// <param name="accessTokenType"></param>
        private void AppendAccessToken(HttpRequestMessage requestMessage, string accessToken, AccessTokenType accessTokenType)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                if (accessTokenType == AccessTokenType.Bearer)
                {
                    //client.SetBearerToken(accessToken);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else if (accessTokenType == AccessTokenType.Basic)
                {
                    var arr = accessToken.Split("@#$");
                    var authenticationString = $"{arr[0]}:{arr[1]}";
                    var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
                }
                else
                {
                    requestMessage.Headers.TryAddWithoutValidation("Authorization", accessToken);
                }
            }
            else
            {
                //lấy access token mặc địch: HttpClien hiện tại đang static
            }
        }

        private async Task<T> ProcessReponseAsync<T>(HttpResponseMessage response) where T : new()
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    var newInstance = Activator.CreateInstance(typeof(T));
                    SetObjectValue(newInstance, "Success", true);
                    return (T)newInstance;

                }
                else
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    catch (Exception)
                    {
                        return (T)Convert.ChangeType(content, typeof(T));
                    }
                }
            }
            else
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response error" + response.StatusCode + "#" + content);
                    var op = JsonConvert.DeserializeObject<T>(content);
                    if (op == null)
                    {
                        var newInstance = Activator.CreateInstance(typeof(T));
                        SetObjectValue(newInstance, "Success", false);

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_UNAUTHORIZED");
                            SetObjectValue(newInstance, "Message", "Xác thực thất bại");
                        }
                        else if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_FORBIDDEN");
                            SetObjectValue(newInstance, "Message", "Không có quyền truy cập API");
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            SetObjectValue(newInstance, "Error", "ERROR_NOT_FOUND");
                            SetObjectValue(newInstance, "Message", "Không tìm thấy API");
                        }

                        return (T)newInstance;
                    }
                }
                catch (Exception)
                {
                    var newInstance = Activator.CreateInstance(typeof(T));
                    SetObjectValue(newInstance, "Success", false);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_UNAUTHORIZED");
                        SetObjectValue(newInstance, "Message", "Xác thực thất bại");
                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_FORBIDDEN");
                        SetObjectValue(newInstance, "Message", "Không có quyền truy cập API");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        SetObjectValue(newInstance, "Error", "ERROR_NOT_FOUND");
                        SetObjectValue(newInstance, "Message", "Không tìm thấy API");
                    }

                    return (T)newInstance;
                }

                var responseStr = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            }

            return default(T);
        }

        private void SetObjectValue(object obj, string prop, object value)
        {
            try
            {
                if (obj != null && obj.GetType().GetProperty(prop) != null)
                {
                    obj.GetType().GetProperty(prop)?.SetValue(obj, value);
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }
        #endregion Private
    }
}
