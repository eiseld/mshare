using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services{
    /// <summary>
    /// Interface defining email sending methods
    /// </summary>
    public interface IEmailService {
        /// <summary>
        /// Send an email to the specified target with the given subject and message
        /// </summary>
        /// <param name="format"></param>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMailAsync(TextFormat format, String name, String target, String subject, String message);
    }
}
