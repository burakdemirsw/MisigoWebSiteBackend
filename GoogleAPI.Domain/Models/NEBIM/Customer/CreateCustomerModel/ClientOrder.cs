using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel
{
    public  class ClientCustomer
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CurrAccCode { get; set; }
        public string? StampPhotoUrl { get; set; }
        public string? BussinesCardPhotoUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
