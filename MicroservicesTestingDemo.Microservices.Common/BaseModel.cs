using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MicroservicesTestingDemo.Microservices.Common
{
    [Index(nameof(IsDeleted))]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public abstract class BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
