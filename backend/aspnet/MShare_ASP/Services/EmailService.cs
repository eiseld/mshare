using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    internal class EmailService : IEmailService {

        private MailboxAddress _senderAddr;
        private Configurations.IEmailConfiguration _emailConf;
        private Configurations.IURIConfiguration _uriConf;

        public EmailService(Configurations.IEmailConfiguration emailConf, Configurations.IURIConfiguration uriConf) {
            _emailConf = emailConf;
            _uriConf = uriConf;
            _senderAddr = new MailboxAddress(_emailConf.Name, _emailConf.Address);
        }

        public async Task SendMailAsync(TextFormat format, String name, String targetEmail, String subject, String message) {
            var msg = new MimeMessage();
            msg.From.Add(_senderAddr);
            msg.To.Add(new MailboxAddress(name, targetEmail));
            msg.Subject = subject;

            msg.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<html><h1><img src='{_uriConf.URIForEndUsers}/assets/MoneyShareLogo_128px.png' alt='MoneyShare logo'></img> MoneyShare</h1><p>{message}</p> </html>"
            };
 
            using (var client = new SmtpClient()) {
                client.ServerCertificateValidationCallback = (s, c, ch, e) => true;

                await client.ConnectAsync(_emailConf.SmtpAddress, _emailConf.SmtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect);

                await client.AuthenticateAsync(_emailConf.Address, _emailConf.Password);

                await client.SendAsync(msg);

                await client.DisconnectAsync(true);
            }
        }
    }
}
