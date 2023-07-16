using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class OrderBillingModel
    {
        // public ItemBillingModel ItemBillingModel { get; set; }
        public string Json { get; set; }

        public decimal TotalValue { get; set; }


    }

    public class OrderBillingListModel
    {
        public List<ItemBillingModel> ItemBillingModels { get; set; }

        public decimal TotalValue { get; set; }


    }
}
