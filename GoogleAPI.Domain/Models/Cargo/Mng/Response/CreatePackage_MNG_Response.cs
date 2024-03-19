﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Cargo.Mng.Response
{
    public class CreatePackage_MNG_Response
    {
        [JsonProperty("orderInvoiceId")]
        public string OrderInvoiceId { get; set; }
        [JsonProperty("orderInvoiceDetailId")]

        public string OrderInvoiceDetailId { get; set; }
        [JsonProperty("shipperBranchCode")]

        public string ShipperBranchCode { get; set; }
    }



}
