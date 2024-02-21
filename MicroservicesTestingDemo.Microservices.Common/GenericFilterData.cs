using Newtonsoft.Json;
using System.Linq.Expressions;

namespace MicroservicesTestingDemo.Microservices.Common
{
    public class GenericFilterData<T> : FilterData
    {
        [JsonIgnore]
        public Expression<Func<T, bool>>? Expression { get; set; }
    }

}
