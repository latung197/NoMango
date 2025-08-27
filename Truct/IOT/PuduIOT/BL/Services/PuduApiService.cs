using PuduIOT.BL.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PuduIOT.BL.Services
{
    public class PuduApiService: IPuduApiService
    {
        private readonly string _apiAppKey;
        private readonly string _apiAppSecret;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public PuduApiService( HttpClient httpClient)
        {

            _apiAppKey = "APIDdPWPWY2EVEPTKWFOd5NNkEpuHRSb9FPjn3n8h";
            _apiAppSecret = "aeipzmPCHWrVthBfbeHDPwnyiNpx32efBB4foCn9Z";
            _baseUrl = "https://css-open-platform.pudutech.com/pudu-entry";
            _httpClient = httpClient;
        }

        public async Task<string> GetDataAsync(string pathAndQuery)
        {
            string url = _baseUrl + pathAndQuery;
            string method = "GET";
            string accept = "application/json";
            string contentType = "application/json";

            // Build signature
            var uri = new Uri(url);
            string path = uri.AbsolutePath + uri.Query;
            var xDate = DateTime.UtcNow.ToString("r");

            string signingStr = $"x-date: {xDate}\n{method}\n{accept}\n{contentType}\n\n{path}";

            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_apiAppSecret));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingStr)));

            var authHeader = $"hmac id=\"{_apiAppKey}\", algorithm=\"hmac-sha1\", headers=\"x-date\", signature=\"{signature}\"";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-date", xDate);
            request.Headers.Add("Authorization", authHeader);
            request.Headers.Accept.ParseAdd(accept);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
