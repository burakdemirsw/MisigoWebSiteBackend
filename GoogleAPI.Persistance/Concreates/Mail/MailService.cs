using GoogleAPI.Domain.Entities;

using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.Abstractions.IServices.IMail;
using Microsoft.Extensions.Configuration;
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
            MailInfo?  mailInfo = _context.msg_MailInfos.FirstOrDefault(m=>m.IsFirst ==true);
            CompanyInfo? companyInfo = _context.msg_CompanyInfos.FirstOrDefault();
            if(mailInfo != null && companyInfo != null)
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = isBodyHtml;
                foreach (string address in addressList)
                    mail.To.Add(address);
                mail.Subject = subject;
                mail.Body = body;
                mail.From = new MailAddress(mailInfo.UserName, companyInfo.CompanyName, Encoding.UTF8);


                SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(
                   mailInfo.UserName,
                    mailInfo.Password
                );
                client.EnableSsl = true;
                await client.SendMailAsync(mail);
            }
           


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
    }
}
