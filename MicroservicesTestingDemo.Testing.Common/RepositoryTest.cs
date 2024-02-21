using AutoMapper;
using MicroservicesTestingDemo.Microservices.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicesTestingDemo.Testing.Common
{
    public abstract class RepositoryTest<T, T1, T2>
            where T : DbContext // DbType (DbContext)
            where T1 : BaseRepository<T, T2> // T2: BaseRepository
            where T2 : BaseModel // EntityType
    {
        protected T ApplicationDbContext { get; set; }
        protected IConfigurationRoot Configuration { get; private set; }
        protected IMapper Mapper { get; set; }
        protected T1 Service { get; set; }
        protected Random Random { get; set; } = new Random();

        public RepositoryTest()
        {
            Configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.testing.json")
                            .Build();
            var options = new DbContextOptionsBuilder<T>();
            var cxn = Configuration.GetConnectionString("TestConnection");
            options.UseSqlServer(cxn);
            var ctor = typeof(T).GetConstructor(new[] { typeof(DbContextOptions<T>) });
            var service_ctor = typeof(T1).GetConstructor(new[] { typeof(T), typeof(ILogger<T1>), typeof(Mapper) });
            ApplicationDbContext = (T)ctor.Invoke(new object[] { options.Options });
            if (ApplicationDbContext.Database.GetPendingMigrations().Any())
                ApplicationDbContext.Database.Migrate();
            var mapperc = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(new List<Profile> {
                    new TestsProfile<T2, T2>(),
                });
            });
            Mapper = new Mapper(mapperc);
            Service = (T1)service_ctor.Invoke(new object[] { ApplicationDbContext, new TestLogger<T1>(), Mapper });
            //MessageBus = new MessageBus(Configuration);
        }
    }

    public class TestLogger<T> : ILogger<T> where T : class
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {

        }
    }

    public class TestsProfile<T, T1> : Profile
    {
        public TestsProfile()
        {
            CreateMap<T, T1>();
            CreateMap<T1, T>();
        }
    }
}