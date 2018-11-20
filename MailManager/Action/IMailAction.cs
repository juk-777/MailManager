using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IMailAction
    {
        bool ActionSend(ConfigEntity configEntity, MailEntity message, int rowNumber);
        bool ActionCopy(ConfigEntity configEntity, MailEntity message, int rowNumber);
        bool ActionPrint(ConfigEntity configEntity, MailEntity message, int rowNumber);
        bool ActionNotify(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
