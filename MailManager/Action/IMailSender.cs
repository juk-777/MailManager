using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IMailSender
    {
        Task<bool> SendAsync(ConfigEntity configEntity, MailEntity message, int rowNumber);
        //void Send(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
