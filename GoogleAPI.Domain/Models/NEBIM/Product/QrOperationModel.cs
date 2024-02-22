using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class QrOperationModel
    {
   
      
        public string? QrBarcode { get; set; }

        
        public string? ShelfNo { get; set; }

        public string? Barcode { get; set; }
 
        public string? BatchCode { get; set; }


       
        public string? ProcessCode { get; set; }
        public int Qty { get; set; }

        public bool IsReturn { get; set; }
    }
    public class QrOperationModel2
    {


        public string? QrBarcode { get; set; }


        public string? ShelfNo { get; set; }

        public string? Barcode { get; set; }

        public string? BatchCode { get; set; }



        public string? ProcessCode { get; set; }
        public int Qty { get; set; }

        public bool IsReturn { get; set; }
        public string? ToWarehouseCode { get; set; }
    }
}
