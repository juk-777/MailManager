using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IPrint
    {
        bool PrintTo(MailEntity message);
    }
}
