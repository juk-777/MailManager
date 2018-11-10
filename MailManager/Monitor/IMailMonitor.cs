using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailMonitor
    {
        void StartMonitor(IList<ConfigEntity> configEntityList);
        void StopMonitor();
        List<string> FirstAccessToMail(ConfigEntity configEntity);
        void OtherAccessToMail(object obj);
        void ProcessingMail(List<MailEntity> messages, ConfigEntity configEntity);
        void DoMailAction(ConfigEntity configEntity, MailEntity message);
    }
}
