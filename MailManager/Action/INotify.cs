using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface INotify
    {
        bool NotifyToAsync(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
