using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface ISaveSeenUids
    {
        bool Save(ConfigEntity configEntity, List<string> seenUids, bool addWrite);
    }
}
