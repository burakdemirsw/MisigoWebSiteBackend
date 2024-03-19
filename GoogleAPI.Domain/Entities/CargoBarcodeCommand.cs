using GoogleAPI.Domain.Entities.Common;
using System;
namespace GoogleAPI.Domain.Entities
{


    public class CargoBarcode : BaseEntity
    { 
        public string? OrderRequest { get; set; }
        public string? OrderResponse { get; set; }
        public string? BarcodeResponse { get; set; }
        public string? OrderNo { get; set; }
        public string? ReferenceId { get; set; }
        public string? BarcodeZplCode { get; set; }
        public string? ShipmentId { get; set; }
        public string? Customer { get; set; }
        public string? BarcodeRequest { get; set; }
        public int? Desi { get; set; }
        public int? Kg { get; set; }
        public int? PackagingType { get; set; }




    }
    public class CargoBarcode_VM 
    {

        public string? OrderNo { get; set; }
        public string? ReferenceId { get; set; }
        public string? BarcodeZplCode { get; set; }
        public string? ShipmentId { get; set; }
        public string? Customer { get; set; }

        public string? BarcodeRequest { get; set; }
        public int? Desi { get; set; }
        public int? Kg { get; set; }
        public int? PackagingType { get; set; }
        public  DateTime? CreatedDate { get; set; }


    }



}
