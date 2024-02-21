using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MicroservicesTestingDemo.Microservices.Common
{
    public abstract class BaseUnitOfWork(IServiceProvider serviceProvider) : IBaseUnitOfWork
    {
        private readonly Lazy<IMapper> mapper = new(serviceProvider.GetRequiredService<IMapper>);
        public IMapper Mapper => mapper.Value;

        public IServiceProvider Services => serviceProvider;

        public ILogger<T> GetLogger<T>() where T : class
        {
            return serviceProvider.GetRequiredService<ILogger<T>>();
        }

        public T GetService<T>()
        {
            return serviceProvider.GetService<T>();
        }
    }

}
