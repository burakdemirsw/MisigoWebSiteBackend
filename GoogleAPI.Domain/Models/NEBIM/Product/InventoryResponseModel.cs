using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public  class InventoryResponseModel
    {
        public string? Barcode { get; set; }
        public string? BatchCode { get; set; }
        public string? WareHouseCode { get; set; }

        public double Quantity { get; set; }

    }
}
