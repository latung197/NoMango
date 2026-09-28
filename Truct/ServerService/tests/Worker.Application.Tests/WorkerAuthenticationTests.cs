using System.Net;
using System.Net.Http.Headers;
using Core.Utils.LogUtils;
using Microsoft.Extensions.Configuration;
using Worker.Application.BaseHttp.Implementations;
using Worker.Application.BaseHttp.Interface;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.Services;
using Xunit;

namespace Worker.Application.Tests;

public sealed class WorkerAuthenticationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ImportRequiresWorkerLoginAndSendsBearerToken(bool loginSucceeds)
    {
        var handler = new FakeHandler(loginSucceeds);
        var client = new BaseHttpClientImpl(new HttpClient(handler));
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ApiDomain"] = "http://localhost",
            ["WorkerUsername"] = "worker",
            ["WorkerPassword"] = "test-password"
        }).Build();
        var service = new WorkerServiceClientImpl(new FakeFactory(client), config, new TestLogger());

        var response = await service.ImportListEcuData(new List<EcuDataDto>());

        Assert.True(response.Code == (loginSucceeds ? "Success" : "Error"), response.Message);
        Assert.Equal(loginSucceeds, handler.ImportCalled);
        if (loginSucceeds) Assert.Equal("test-token", handler.Authorization?.Parameter);
    }

    private sealed class FakeFactory(IBaseHttpClient client) : IBaseHttpClientFactory
    {
        public IBaseHttpClient Create() => client;
    }

    private sealed class FakeHandler(bool loginSucceeds) : HttpMessageHandler
    {
        public bool ImportCalled { get; private set; }
        public AuthenticationHeaderValue? Authorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("/login", StringComparison.Ordinal))
                return Task.FromResult(Json(loginSucceeds
                    ? "{\"code\":\"Success\",\"data\":{\"token\":\"test-token\"}}"
                    : "{\"code\":\"Error\"}"));

            ImportCalled = true;
            Authorization = request.Headers.Authorization;
            return Task.FromResult(Json("{\"code\":\"Success\"}"));
        }

        private static HttpResponseMessage Json(string content) => new(HttpStatusCode.OK)
        {
            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        };
    }

    private sealed class TestLogger : ILoggerManager
    {
        public void LogInfo(string message) { }
        public void LogInfo(object data) { }
        public void LogDebug(object data) { }
        public void LogError(object data) { }
        public void LogWarning(object data) { }
        public void LogTrace(object data) { }
        public void LogDebug(string message) { }
        public void LogError(string message) { }
        public void LogError(Exception ex, string message) { }
        public void LogError(Exception ex) { }
        public void LogWarning(string message) { }
        public void LogTrace(string message) { }
    }
}
