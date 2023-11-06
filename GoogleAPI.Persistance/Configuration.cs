using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Persistance
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                //ConfigurationManager configurationManager = new();
                //configurationManager.SetBasePath(
                //    Path.Combine(Directory.GetCurrentDirectory(), "../../GoogleAPI/GoogleAPI.API")
                //);
                //configurationManager.AddJsonFile("appsettings.json");

                return "Server=(localdb)\\MSSQLLocalDB;Database=WhatsappAPI;trusted_connection=true";
            }
        }
    }
}
