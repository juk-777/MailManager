using MailManager.Config;
using System;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailProvider
    {
        void GetAllMessages(ConfigEntity configEntity, out List<MailEntity> allMessages, out List<string> allUids);
        List<MailEntity> GetUnseenMessages(ConfigEntity configEntity, List<string> seenUids, out List<string> seenUidsNew);
    }
}
