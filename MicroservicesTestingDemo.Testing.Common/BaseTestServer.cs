using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using Xunit.Abstractions;
using Xunit.Extensions.Logging;
using Xunit.Sdk;

namespace MicroservicesTestingDemo.Testing.Common
{
    public abstract class BaseTestServer<T> where T : class
    {
        protected IWebHostBuilder builder;
        protected IConfigurationRoot configuration;
        public TestServer? Server { get; set; }
        public HttpClient? Client { get; set; }
        public ITestOutputHelper TestOutputHelper { get; }

        protected BaseTestServer(string? appsettings = null, bool isTesting = false)
        {
            builder = new WebHostBuilder().UseStartup<T>().UseEnvironment("Test");
            var cb = new ConfigurationBuilder();
            var defaults = new[] { "appsettings.Testing.json", "appsettings.json" };
            if (appsettings != null)
                cb.AddJsonFile(appsettings);
            foreach (var defaultd in defaults)
            {
                if (File.Exists(defaultd))
                {
                    cb.AddJsonFile($"{defaultd}");
                    break;
                }
            }
            configuration = cb.Build();
            builder.UseConfiguration(configuration);
            CreateServerAndClient();
            TestOutputHelper = new TestOutputHelper();
            builder.ConfigureLogging(o =>
            {
                o.ClearProviders();
                o.Services.AddSingleton<ILoggerProvider>(new XunitLoggerProvider(TestOutputHelper, configuration));
            });
        }

        protected void CreateServerAndClient()
        {
            Server ??= new TestServer(builder)
            {
                PreserveExecutionContext = true,
                AllowSynchronousIO = true
            };
            Server.AllowSynchronousIO = true;
            Client = Server.CreateClient();
        }
    }
}
