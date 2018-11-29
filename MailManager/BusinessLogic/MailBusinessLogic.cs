using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailManager.Config;
using System.Threading;
using MailManager.Monitor;

namespace MailManager.BusinessLogic
{
    public class MailBusinessLogic : IMailBusinessLogic
    {
        private readonly IConfigReader _configReader;
        private readonly IConfigVerify _configVerify;
        private readonly IMailMonitor _mailMonitor;
        
        public MailBusinessLogic(IConfigReader configReader, IConfigVerify configVerify, IMailMonitor mailMonitor)
        {
            _configReader = configReader;
            _configVerify = configVerify;
            _mailMonitor = mailMonitor;
        }
        public async Task StartJob(CancellationToken token)
        {
            Console.WriteLine("\nНачинаю работу ...");

            if (token.IsCancellationRequested)
                return;

            Console.WriteLine("\nСчитывание конфигурации ...");
            List<ConfigEntity> configEntityList = await Task.Run(() => _configReader.ReadConfig(), token);

            if (configEntityList == null || configEntityList.Count == 0)
                throw new ArgumentException("Файл конфигурации пуст!");

            Console.WriteLine("\nПроверка конфигурации ...");
            if (! _configVerify.VerifyConfig(configEntityList))
                throw new ArgumentException("Проверка завершена с ошибкой!");

            Console.WriteLine("\nЗапускаем мониторинг почты ...");
            await Task.Run(()=> _mailMonitor.StartMonitor(configEntityList), token);
        }

        #region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mailMonitor.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
