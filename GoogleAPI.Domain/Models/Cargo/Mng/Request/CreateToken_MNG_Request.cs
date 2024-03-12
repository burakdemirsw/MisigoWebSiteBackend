using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Cargo.Mng.Request
{
    public class CreateToken_MNG_Request
    {
        public string CustomerNumber { get; set; }
        public string Password { get; set; }
        public int IdentityType { get; set; }

    }
}
