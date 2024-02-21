using Microsoft.Extensions.Configuration;

namespace MicroservicesTestingDemo.Testing.Common
{
    public class BaseHttpAPITests<T, T1> : BaseTestServer<T>, ITestServerAndClient<T1>, IDisposable
    where T : class
    where T1 : class
    {
        public string BaseUrl { get; set; }
        public T1 ApiClient { get; }

        public BaseHttpAPITests(string? appsettings = null, bool isTesting = false) : base(appsettings, isTesting)
        {
            if (Client == null)
                CreateServerAndClient();
            if (Client == null) throw new NullReferenceException(nameof(Client));
            if (Client.BaseAddress == null) throw new NullReferenceException(nameof(Client.BaseAddress));
            BaseUrl = Client.BaseAddress.ToString();
            var type = typeof(T1);
            var ctor = type.GetConstructor([typeof(HttpClient), typeof(IConfiguration), typeof(IServiceProvider)]);
            if (ctor == null)
            {
                throw new NullReferenceException(nameof(ctor));
            }
            ApiClient ??= (T1)ctor.Invoke(new object[] { Client, null, null });
        }

        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
