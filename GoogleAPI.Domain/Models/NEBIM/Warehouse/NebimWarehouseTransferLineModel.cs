using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class NebimWarehouseTransferLineModel
    {
        public int ItemTypeCode { get; set; }
        public string UsedBarcode { get; set; }
        public string ItemCode { get; set; }
        public string ColorCode { get; set; }
        public string ItemDim1Code { get; set; }
        public string BatchCode { get; set; }
        public int? Qty1 { get; set; }
        public List<NebimWarehouseTransferITAttributeModel> ITAttributes { get; set; }
    }
}
