using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IPrint
    {
        bool PrintTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
