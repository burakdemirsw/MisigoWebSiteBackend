﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CountListModel
    {
        public DateTime? LastUpdateDate { get; set; }
        public int? TotalProduct { get; set; }
        public string? OrderNo{ get; set; }
    }
}