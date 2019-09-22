using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{

    /// <summary>Email related services</summary>
    public interface IEmailService
    {

        /// <summary>Send an email to the specified target with the given subject and message</summary>
        Task SendMailAsync(TextFormat format, String name, String target, String subject, String message);
    }
}
