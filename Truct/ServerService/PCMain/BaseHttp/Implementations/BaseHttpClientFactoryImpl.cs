using Microsoft.Extensions.DependencyInjection;
using PCMain.BaseHttp.Interface;
using Worker.Application.Constants;

namespace PCMain.BaseHttp.Interface
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
