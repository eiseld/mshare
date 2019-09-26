using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MShare_ASP.Configurations;

namespace MShare_ASP.Services
{
    internal class EmailService : IEmailService
    {
        private MailboxAddress SenderAddr { get; }
        private IEmailConfiguration EmailConf { get; }
        private IURIConfiguration UriConf { get; }

        public EmailService(IEmailConfiguration emailConf, IURIConfiguration uriConf)
        {
            EmailConf = emailConf;
            UriConf = uriConf;
            SenderAddr = new MailboxAddress(EmailConf.Name, EmailConf.Address);
        }

        public async Task SendMailAsync(TextFormat format, String name, String targetEmail, String subject, String message)
        {
            var msg = new MimeMessage();
            msg.From.Add(SenderAddr);
            msg.To.Add(new MailboxAddress(name, targetEmail));
            msg.Subject = subject;

            msg.Body = new TextPart(format)
            {
                Text = message
            };
             
            using (var client = new SmtpClient()) 
            {
                client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                await client.ConnectAsync(EmailConf.SmtpAddress, EmailConf.SmtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect);

                await client.AuthenticateAsync(EmailConf.Address, EmailConf.Password);

                await client.SendAsync(msg);

                await client.DisconnectAsync(true);
            }
        }
    }
}
