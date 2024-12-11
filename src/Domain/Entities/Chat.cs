using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{    
    public sealed class Chat : BaseEntity, IAggregateRoot
    {
        public int CreatorId { get; set; }  
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public required string Message { get; set; } // JSONB Type
    }
}
