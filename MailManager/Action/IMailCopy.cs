using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IMailCopy
    {
        Task<bool> CopyToAsync(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
