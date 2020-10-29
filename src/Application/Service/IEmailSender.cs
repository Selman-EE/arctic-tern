using Application.Model;
using System.Threading.Tasks;

namespace Application.Service
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}
