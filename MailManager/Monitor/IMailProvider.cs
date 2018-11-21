using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailProvider
    {
        MailTransfer GetAllMessages(ConfigEntity configEntity);
        MailTransfer GetUnseenMessages(ConfigEntity configEntity, List<string> seenUids);
    }
}
