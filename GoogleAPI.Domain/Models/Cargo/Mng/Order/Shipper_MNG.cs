namespace GoogleAPI.Domain.Models.Cargo.Mng.Order
{
    public class Shipper_MNG
    {
        public int CustomerId { get; set; }
        public string RefCustomerId { get; set; }
        public int CityCode { get; set; }
        public int DistrictCode { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string Address { get; set; }
        public string BussinessPhoneNumber { get; set; }
        public string Email { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string FullName { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
    }

}
