using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IPrint
    {
        bool PrintTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
