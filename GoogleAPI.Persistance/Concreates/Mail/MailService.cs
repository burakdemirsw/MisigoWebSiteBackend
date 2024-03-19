using GoogleAPI.Domain.Entities;

using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions.IServices.IMail;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace GoogleAPI.Persistance.Concreates.Services.Mail
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;
        private readonly GooleAPIDbContext _context;
        public MailService(IConfiguration configuration , GooleAPIDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task SendMail(List<string> addressList, string subject, string body, bool isBodyHtml)
        {
            //MailInfo?  mailInfo = _context.msg_MailInfos.FirstOrDefault(m=>m.IsFirst ==true);
            //CompanyInfo? companyInfo = _context.msg_CompanyInfos.FirstOrDefault();

            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = isBodyHtml;
            foreach (string address in addressList)
                mail.To.Add(address);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress("demir-burock@hotmail.com", "Davye", Encoding.UTF8);


            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(
              "demir-burock@hotmail.com", "Marazali00."
            );
            client.EnableSsl = true;
            await client.SendMailAsync(mail);



        }

        public async Task SendPasswordResetEmail(string email, string userId, string resetToken)
        {
            CompanyInfo? companyInfo = _context.msg_CompanyInfos.FirstOrDefault();
            if ( companyInfo != null)
            {

                var returnUrl = $"{companyInfo.PasswordResetUrl}/{userId}/{resetToken}";
                var mail = await File.ReadAllTextAsync("C://code//resetPasswordEmail.txt");
                mail = mail.Replace("[LOGOURL]", companyInfo.LogoUrl).Replace("[WEBSITEURL]", companyInfo.WebSiteUrl).Replace("[RETURNURL]", returnUrl);
                await SendMail(new List<string>() { email }, "Şifre Yenileme Talebi", mail, true);
            }

        }

        public async Task SendCargoInfoMail(List<string> emails,string orderNo, string cargoUrl,int state)
        {
            emails.Add("bilgi@davye.com");
            
            string infoMessage ="";
            string header = "";
            if (state == 0)
            {
                header = "Oluşturuldu";
                infoMessage = $"Sevgili Müşterimiz! {orderNo} Numaralı siparişiniz oluşturuldu. Siparişinizin bir sonraki adımı hakkında sizi bilgilendirmeye devam edeceğiz. Sabrınız ve anlayışınız için teşekkür ederiz!";
            }
            if (state == 1)
            {
                header = "Paketlendi";
                infoMessage = $"Sevgili Müşterimiz! {orderNo} Numaralı siparişiniz özenle paketlendi ve kargoya teslim edilmek üzere hazırlanıyor. Siparişinizin bir sonraki adımı hakkında sizi bilgilendirmeye devam edeceğiz. Sabrınız ve anlayışınız için teşekkür ederiz!";
            }
            if (state ==2)
            {
                header = "Yola Çıktı";
                infoMessage = $"Sevgili Müşterimiz,{orderNo} Numaralı siparişiniz kargoya verildi ve yola çıktı! Gönderinizin durumunu takip etmek için size özel takip numaranızı kullanabilirsiniz.  Sabrınız ve anlayışınız için teşekkür ederiz!";
            }
            if (state == 3)
            {
                header = "Dağıtımda";
                infoMessage = $"Sevgili Müşterimiz,{orderNo} Numaralı siparişiniz dağıtımda ve yola çıktı! Gönderinizin durumunu takip etmek için size özel takip numaranızı kullanabilirsiniz.  Sabrınız ve anlayışınız için teşekkür ederiz!";
            }
            if (state ==4)
            {
                header = "Teslim Edildi";
                infoMessage = $"Sevgili Müşterimiz,{orderNo} Nuramarlı siparişiniz teslim edildi ve yola çıktı! Bizi tercih ettiğiniz için teşekkür ederiz!";
            }
            var mail = await File.ReadAllTextAsync("C://code//cargoInfoEmail.txt");
            mail = mail.Replace("[CARGOURL]", cargoUrl).Replace("[CargoInfo]", infoMessage).Replace("[STATE]", header);
            await SendMail(emails, $"SİPARİŞİNİZ {header}", mail, true);

        }

    }
}
