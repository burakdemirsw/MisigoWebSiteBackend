using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel
{

    public class AddCustomerAddress_CM
    {
        public int ModelType { get; set; }
        public string CurrAccCode { get; set; }
        public List<PostalAddress> PostalAddresses { get; set; }
    }
}
