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

        public bool ActionSend(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            return _mailSender.SendTo(configEntity, message, rowNumber);
        }

        public bool ActionCopy(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            return _mailCopy.CopyTo(configEntity, message, rowNumber);
        }

        public bool ActionPrint(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            return _print.PrintTo(configEntity, message, rowNumber);
        }

        public bool ActionNotify(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            return _notify.NotifyTo(configEntity, message, rowNumber);
        }
         
    }
}
