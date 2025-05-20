using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PlastMB.Helper
{
    public class WebRequestHelper
    {
        private readonly HttpClient _httpClient;
        public static string Hostname = "127.0.0.1";
        public static string Port = "2511";
        public static string Path = "/api/TrnImportHistory/";


        // Hàm tạo URL từ Hostname, Port và Path
        public string AgentEntryPoint()
        {
            return $"http://{Hostname}:{Port}{Path}";
        }

        public WebRequestHelper()
        {
          
            _httpClient = new HttpClient();


            // Nếu có token, thêm vào header
            //if (!string.IsNullOrEmpty(Token))
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization =
            //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            //}
        }

        // Phương thức GetAsync sử dụng path
        public async Task<string> GetAsync(string path)
        {
            Path = path;
            var url = AgentEntryPoint();
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Phương thức PostAsync sử dụng path
        public async Task<string> PostAsync(string path, string jsonContent)
        {
            Path = path;
            var url = AgentEntryPoint();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Phương thức PutAsync sử dụng path
        public async Task<string> PutAsync(string path, string jsonContent)
        {
            Path = path;
            var url = AgentEntryPoint();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Phương thức DeleteAsync sử dụng path
        /*public async Task<bool> DeleteAsync(string path)
        {
            Path = path;
            var url = AgentEntryPoint();
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }*/

        // Phương thức DeleteAsync sử dụng path
        public async Task<string> DeleteAsync(string path)
        {
            Path = path;
            var url = AgentEntryPoint();
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
