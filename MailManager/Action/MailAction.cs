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

        public bool ActionSend(ConfigEntity configEntity, MailEntity message, string mailActionValue)
        {
            return _mailSender.SendTo(configEntity, message, mailActionValue);
        }

        public bool ActionCopy(ConfigEntity configEntity, MailEntity message, string mailActionValue)
        {
            return _mailCopy.CopyTo(configEntity, message, mailActionValue);
        }

        public bool ActionPrint(MailEntity message)
        {
            return _print.PrintTo(message);
        }

        public bool ActionNotify(MailEntity message)
        {
            return _notify.NotifyTo(message);
        }
         
    }
}
