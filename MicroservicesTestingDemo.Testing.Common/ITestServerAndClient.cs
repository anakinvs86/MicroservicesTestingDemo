using Microsoft.AspNetCore.TestHost;

namespace MicroservicesTestingDemo.Testing.Common
{
    public interface ITestServerAndClient<T>
    {
        HttpClient Client { get; }
        TestServer Server { get; }
        T ApiClient { get; }
    }
}
