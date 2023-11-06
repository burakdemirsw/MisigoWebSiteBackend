using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAPI.Domain.Entities.Common;

namespace GoogleAPI.Domain.Entities
{
    public class Photo : BaseEntity
    {
        public string Url { get; set; }
        ICollection<Product> Products { get; set; }
    }
}
