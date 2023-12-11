using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Filter
{
    public class WarehouseTransferListFilterModel
    {
        public string? OrderNumber { get; set; }
        public string? WarehouseCode { get; set; }
        public string? ToWareHouseCode { get; set; }
        public DateTime? OperationStartDate { get; set; }
        public DateTime? OperationEndDate { get; set; }
    }


}
