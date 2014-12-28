using System.Net.Mail;

namespace NBlog.Web.Application.Service.Internal
{
    public class MessageService : IMessageService
    {
        public void SendEmail(MailMessage mailMessage)
        {
            new SmtpClient("nonov.net", 25).Send(mailMessage);
        }
    }
}