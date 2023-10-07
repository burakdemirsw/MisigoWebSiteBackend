using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Filter
{
    public class WarehouseOperationListFilterModel
    {
        public string? InnerNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
