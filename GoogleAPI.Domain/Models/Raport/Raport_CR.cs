using GoogleAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Raport
{
    public class Raport_CR : BaseEntity
    {
        public Raport_1? Raport_1 { get; set; }
        public List<Raport_2>? Raport_2 { get; set; }
        public List<Raport_3>? Raport_3 { get; set; }
        public List<Raport_4>? Raport_4 { get; set; }


    }
    public class Raport_1 

    {
        public int WSOrderCount { get; set; }
        public int WSOrderRevenue { get; set; }
        public int CustomerCount { get; set; }


    }
    public class Raport_2
    {
        public string Day { get; set; }
        public int OrderCount { get; set; }

    }
    public class Raport_3
    {
        public string Day { get; set; }
        public int OrderRevenue { get; set; }

    }
    public class Raport_4
    {
        public string PhotoUrl { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public int SaleCount { get; set; }
        public decimal Revenue { get; set; }

    }
}
