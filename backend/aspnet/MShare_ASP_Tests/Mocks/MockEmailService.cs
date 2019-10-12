using MimeKit.Text;
using MShare_ASP.Services;
using System.Threading.Tasks;

namespace MShare_ASP_Tests.Mocks
{
    public class MockEmailService : IEmailService
    {
        public Task SendMailAsync(TextFormat format, string name, string target, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}