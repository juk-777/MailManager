using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IMailAction
    {
        bool ActionSend(ConfigEntity configEntity, MailEntity message, string mailActionValue);
        bool ActionCopy(ConfigEntity configEntity, MailEntity message, string mailActionValue);
        bool ActionPrint(MailEntity message);
        bool ActionNotify(MailEntity message);
    }
}
