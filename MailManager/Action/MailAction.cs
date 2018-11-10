using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class MailAction : IMailAction
    {
        private readonly IMailSender _mailSender;
        private readonly IMailCopy _mailCopy;
        private readonly INotify _notify;
        private readonly IPrint _print;

        public MailAction(IMailSender mailSender, IMailCopy mailCopy, INotify notify, IPrint print)
        {
            _mailSender = mailSender;
            _mailCopy = mailCopy;
            _notify = notify;
            _print = print;
        }

        public void ActionSendAsync(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _mailSender.SendAsync(configEntity, message, rowNumber);
        }

        public void ActionCopy(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _mailCopy.CopyTo(configEntity, message, rowNumber);
        }

        public void ActionPrint(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _print.PrintTo(configEntity, message, rowNumber);
        }

        public void ActionNotify(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _notify.NotifyTo(configEntity, message, rowNumber);
        }
         
    }
}
