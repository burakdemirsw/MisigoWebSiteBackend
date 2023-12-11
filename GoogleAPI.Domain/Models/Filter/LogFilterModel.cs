using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Filter
{
    public class LogFilterModel
    {
        public int Id { get; set; }
        public string? MessageHeader { get; set; }
        public string? Level { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
