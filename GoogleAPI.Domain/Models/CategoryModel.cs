using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? TopCategory { get; set; }
        public string? SubCategory { get; set; }
        public string? SubCategory2 { get; set; }
        public string? SubCategory3 { get; set; }
        public string? SubCategory4 { get; set; }
        public string? SubCategory5 { get; set; }
    }

}
