using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface ISendWork
    {
        bool DoWork(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
