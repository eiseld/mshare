using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    interface IEmailService {
        Task SendMailAsync(TextFormat format, String name, String target, String subject, String message);
    }
}
