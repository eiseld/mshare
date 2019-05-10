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

        public EmailService(Configurations.IEmailConfiguration emailConf) {
            _emailConf = emailConf;
            _senderAddr = new MailboxAddress(_emailConf.Name, _emailConf.Address);
        }

        public async Task SendMailAsync(TextFormat format, String name, String targetEmail, String subject, String message) {
            var msg = new MimeMessage();
            msg.From.Add(_senderAddr);
            msg.To.Add(new MailboxAddress(name, targetEmail));
            msg.Subject = subject;

            var filePath = "./Resources/MoneyShareLogo_128px.png";
            var logo = new MimePart("image", "png")
            {
                Content = new MimeContent(System.IO.File.OpenRead(filePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = System.IO.Path.GetFileName(filePath),
                ContentId="logoImage"
            };

            msg.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<html><h1><img src='{logo.ContentId}' alt='MoneyShare logo'></img> MoneyShare</h1><p>{message}</p> </html>"
            };
            
            var multipart = new Multipart("mixed");
            multipart.Add(logo);
            multipart.Add(msg.Body);

            msg.Body = multipart;

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
