using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

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

        public void ActionSend(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _mailSender.SendAsync(configEntity, message, rowNumber);
        }

        public void ActionCopy(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _mailCopy.CopyToAsync(configEntity, message, rowNumber);
        }

        public void ActionPrint(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            _print.PrintToAsync(configEntity, message, rowNumber);
        }

        public async void ActionNotify(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            //_notify.NotifyToAsync(configEntity, message, rowNumber);
            await Task.Run(() => _notify.NotifyToAsync(configEntity, message, rowNumber));
        }
         
    }
}
