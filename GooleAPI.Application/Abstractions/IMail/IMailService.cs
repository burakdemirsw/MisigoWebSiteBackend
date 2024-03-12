using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions.IServices.IMail
{
    public interface IMailService
    {
        Task SendMail(List<string> addressList , string subject , string body, bool isBodyHtml    );

        Task SendPasswordResetEmail(string email,string userId,string resetToken);

    }
}
