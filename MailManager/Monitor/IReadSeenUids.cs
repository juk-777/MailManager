using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IReadSeenUids
    {
        List<string> Read(ConfigEntity configEntity);
    }
}
