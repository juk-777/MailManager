using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IMailCopy
    {
        bool CopyTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
