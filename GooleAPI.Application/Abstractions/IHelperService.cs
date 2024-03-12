using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions.IServices.IHelper
{
    public  interface IHelperService
    {
        string ComputeHMACSHA256(string data, string key);
       bool VerifyHMACSHA256(string originalData, string hashedData, string key);
    }
}
