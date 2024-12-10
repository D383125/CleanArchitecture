using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    
    public sealed class Product : BaseEntity
    {        
        public required string Name { get; set; }

        public decimal Price { get; set; }
    }
}
