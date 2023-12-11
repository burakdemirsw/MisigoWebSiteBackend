using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class CollectAndPackModel
    {
        public string? Barcode { get; set; }
        public string? ShelfNo { get; set; }
        public string? OrderNo { get; set; }
    }
}
