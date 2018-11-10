using MailManager.Config;
using System;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailProvider : IDisposable
    {
        void Initialize(ConfigEntity configEntity);
        void Connect();
        void Disconnect();
        void GetAllMessages(out List<MailEntity> allMessages, out List<string> allUids);
        List<MailEntity> GetUnseenMessages(List<string> seenUids, out List<string> seenUidsNew);
    }
}
