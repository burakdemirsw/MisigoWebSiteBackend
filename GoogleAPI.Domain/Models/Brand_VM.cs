using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class Brand_VM
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }

    }
}
