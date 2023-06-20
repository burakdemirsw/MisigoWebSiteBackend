using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.ViewModels
{
    public class VM_Update_Product
    {
        public int Id { get; set; }
        public string? StockCode { get; set; }
        public string? Barcode { get; set; }
        public string? ProductName { get; set; }
    }
}
