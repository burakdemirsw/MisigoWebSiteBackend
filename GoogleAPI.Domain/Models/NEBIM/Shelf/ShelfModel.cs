using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Shelf
{
    public class ShelfModel
    {

        public int Id { get; set; }
        public int? PackageId { get; set; }
        public string? QrString { get; set; }
        public string? Warehouse { get; set; }
        public string? ShelfNo { get; set; }
        public string? ItemCode { get; set; }
        public string? Party { get; set; }
        public int? Inventory { get; set; }
    }

    public class AvailableShelf
    {


        public string? Description { get; set; }
      
    }
}
