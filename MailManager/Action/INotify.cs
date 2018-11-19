using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface INotify
    {
        bool NotifyTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
