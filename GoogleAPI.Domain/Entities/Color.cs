using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAPI.Domain.Entities.Common;

namespace GoogleAPI.Domain.Entities
{
    public class Color : BaseEntity
    {
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; } // Renk ile ilişkiyi temsil eden navigation property

    }
}
