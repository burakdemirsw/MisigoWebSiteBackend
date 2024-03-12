using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel
{
    public class CreateCustomer_CM
    {
        public string? OfficeCode { get; set; }//++
        public string? WarehouseCode { get; set; }//++
        public string? CurrAccDescription { get; set; }
        public string? TaxOfficeCode { get; set; }
        public string? MersisNum { get; set; }
        public string? Mail { get; set; } //++
        public string? PhoneNumber { get; set; } //++
        public string? TaxNumber { get; set; } //++
        public string? FirmDescription { get; set; }//++
        public string? StampPhotoUrl { get; set; }//++
        public string? BussinesCardPhotoUrl { get; set; }//++
        public Address? Address { get; set; }

    }

    public class Address
    {
        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Region { get; set; }
        public string? TaxOffice { get; set; }
        public string? Description { get; set; }
        public string? PostalCode { get; set; }
    }

    public class CreateCustomer_ResponseModel
    {
        public string? CurrAccCode { get; set; }//++


    }


}
