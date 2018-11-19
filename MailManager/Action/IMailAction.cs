using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IMailAction
    {
        //Task ActionSendEmailAsync(ConfigEntity configEntity, MailEntity message, int rowNumber);
        void ActionSend(ConfigEntity configEntity, MailEntity message, int rowNumber);
        void ActionCopy(ConfigEntity configEntity, MailEntity message, int rowNumber);
        void ActionPrint(ConfigEntity configEntity, MailEntity message, int rowNumber);
        void ActionNotify(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
