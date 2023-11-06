using GoogleAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Entities
{
    public class Brand :BaseEntity
    {
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
