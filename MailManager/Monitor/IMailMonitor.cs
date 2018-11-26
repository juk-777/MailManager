using System;
using MailManager.Config;
using System.Collections.Generic;

namespace MailManager.Monitor
{
    public interface IMailMonitor : IDisposable
    {
        void StartMonitor(List<ConfigEntity> configEntityList);
    }
}
