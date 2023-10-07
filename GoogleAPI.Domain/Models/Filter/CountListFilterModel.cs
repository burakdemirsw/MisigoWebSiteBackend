using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Filter
{
    public class CountListFilterModel
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? TotalProduct { get; set; }
        public string? OrderNo { get; set; }


    }
}
