using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface ISeenUidsManager
    {
        bool Write(ConfigEntity configEntity, List<string> seenUids, bool addWrite);
        List<string> Read(ConfigEntity configEntity);
    }
}
