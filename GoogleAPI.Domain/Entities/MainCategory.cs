using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAPI.Domain.Entities.Common;

namespace GoogleAPI.Domain.Entities
{
    public class MainCategory : BaseEntity
    {
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
