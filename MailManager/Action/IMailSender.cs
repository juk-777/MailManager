using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IMailSender
    {
        bool SendTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
