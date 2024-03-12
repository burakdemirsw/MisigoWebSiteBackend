using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Customer
{


    public class NebimCustomer
    {
        public int ModelType { get; set; }
        public string CurrAccCode { get; set; }

        public string CurrAccDescription { get; set; }
        public string TaxNumber { get; set; }
        public string TaxOfficeCode { get; set; }
        public string MersisNum { get; set; }

        public string OfficeCode { get; set; }
        public string RetailSalePriceGroupCode { get; set; }
        public string IdentityNum { get; set; } //11111111111
        public int CreditLimit { get; set; } //0


        public string CurrencyCode { get; set; } //TRY
        public List<PostalAddress> PostalAddresses { get; set; } // spden gelcek
        public List<Communication> Communications { get; set; }


    }


    public class NebimCustomer_Retail
    {
        public int ModelType { get; set; }
        public string CurrAccCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OfficeCode { get; set; }
        public string RetailSalePriceGroupCode { get; set; }
        public string IdentityNum { get; set; } //11111111111
        public int CreditLimit { get; set; } //0

      
        public string CurrencyCode { get; set; } //TRY
        public List<PostalAddress> PostalAddresses { get; set; } // spden gelcek
        public List<Communication> Communications { get; set; }


    }
    public class NebimCustomer_V2
    {
        public int ModelType { get; set; }
        public string CurrAccCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OfficeCode { get; set; }
        public string RetailSalePriceGroupCode { get; set; }
        public string IdentityNum { get; set; } //11111111111
        public int CreditLimit { get; set; } //0


        public string CurrencyCode { get; set; } //TRY
        //public List<PostalAddress> PostalAddresses { get; set; } // spden gelcek
        public List<Communication> Communications { get; set; }


    }
}
