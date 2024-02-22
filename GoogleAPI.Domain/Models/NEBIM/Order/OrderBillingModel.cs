using GoogleAPI.Domain.Models.NEBIM.Invoice;

namespace GoogleAPI.Domain.Models.NEBIM.Order
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
