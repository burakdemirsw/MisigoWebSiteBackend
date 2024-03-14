using GoogleAPI.Domain.Entities.Common;
using System;
namespace GoogleAPI.Domain.Entities
{


    public class CargoBarcode : BaseEntity
    { 
        public string? Request { get; set; }
        public string? Response { get; set; }
        public string? OrderNo { get; set; }
        public string? ReferenceId { get; set; }
        public string? BarcodeZplCode { get; set; }
        public string? ShipmentId { get; set; }


    }



}
