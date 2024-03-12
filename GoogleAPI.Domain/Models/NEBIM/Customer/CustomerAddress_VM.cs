namespace GoogleAPI.Domain.Models.NEBIM.Customer
{
    public class CustomerAddress_VM
    {
        public Guid PostalAddressID { get; set; }
        public string? CurrAccCode { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryDescription { get; set; }
        public string? StateCode { get; set; }
        public string? StateDescription { get; set; }
        public string? CityCode { get; set; }
        public string? CityDescription { get; set; }
        public string? DistrictCode { get; set; }
        public string? DistrictDescription { get; set; }
        public string? Address { get; set; }

    }
}
