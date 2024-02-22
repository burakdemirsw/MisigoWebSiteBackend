using Microsoft.Extensions.Configuration;

namespace GoogleAPI.Persistance
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(
                    Path.Combine(Directory.GetCurrentDirectory(), "../../GoogleAPI/GoogleAPI.API")
                );
                configurationManager.AddJsonFile("appsettings.json");
                var sqlCon1 = "Data Source=192.168.2.36;Initial Catalog=BDD2017;User ID=sa;Password=8969;TrustServerCertificate=True;";
                var sqlCon2 = "Data Source=192.168.2.38;Initial Catalog=MISIGO;User ID=sa;Password=1524;TrustServerCertificate=True;";
                return sqlCon1;
            }
        }
    }
}
