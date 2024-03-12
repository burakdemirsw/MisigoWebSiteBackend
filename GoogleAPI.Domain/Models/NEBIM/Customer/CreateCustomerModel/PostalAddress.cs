namespace GoogleAPI.Domain.Models.NEBIM.Customer
{
    public class PostalAddress
    {
        public string AddressTypeCode { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string CityCode { get; set; }
        public string DistrictCode { get; set; }
        public string Address { get; set; }
    }
}
