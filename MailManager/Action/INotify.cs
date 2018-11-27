using MailManager.Monitor;

namespace MailManager.Action
{
    public interface INotify
    {
        bool NotifyTo(MailEntity message);
    }
}
