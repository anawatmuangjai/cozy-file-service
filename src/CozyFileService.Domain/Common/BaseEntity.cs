using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyFileService.Domain.Common
{
    public abstract class BaseEntity
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
