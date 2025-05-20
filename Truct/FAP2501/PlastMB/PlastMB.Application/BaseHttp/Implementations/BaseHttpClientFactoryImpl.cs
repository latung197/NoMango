
using PlastMB.Application.BaseHttp.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace PlastMB.Application.BaseHttp.Implementations
{
    public class BaseHttpClientFactoryImpl : IBaseHttpClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseHttpClientFactoryImpl(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IBaseHttpClient Create()
        {
            return _serviceProvider.GetRequiredService<IBaseHttpClient>();
        }
    }
}
