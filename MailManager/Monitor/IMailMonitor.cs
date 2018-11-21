using System;
using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailMonitor : IDisposable
    {
        void StartMonitor(List<ConfigEntity> configEntityList);
        List<string> FirstAccessToMail(ConfigEntity configEntity);
        void OtherAccessToMail(object obj);
        void ProcessingMail(List<MailEntity> messages, ConfigEntity configEntity);
        void DoMailActionAsync(ConfigEntity configEntity, MailEntity message);
    }
}
