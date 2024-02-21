using AutoMapper;
using Microsoft.Extensions.Logging;

namespace MicroservicesTestingDemo.Microservices.Common
{
    public interface IBaseUnitOfWork
    {
        IServiceProvider Services { get; }

        ILogger<T> GetLogger<T>() where T : class;

        IMapper Mapper { get; }

        T GetService<T>();
    }

}
