using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IMailCopy
    {
        bool CopyTo(ConfigEntity configEntity, MailEntity message, string mailActionValue);
    }
}
