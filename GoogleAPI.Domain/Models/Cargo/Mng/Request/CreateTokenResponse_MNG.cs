using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Cargo.Mng.Request
{
    public class CreateTokenResponse_MNG
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
        public string JwtExpireDate { get; set; }
        public string RefreshTokenExpireDate { get; set; }
    }
}
