using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class GetRemainingsProductsRequest
    {
        public string PackageId { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
    }



}
