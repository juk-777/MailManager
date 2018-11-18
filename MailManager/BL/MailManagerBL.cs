using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailManager.Config;
using System.Threading;
using MailManager.Monitor;

namespace MailManager.BL
{
    public class MailManagerBL : IMailManagerBL
    {
        private readonly IConfigReader _configReader;
        private readonly IMailMonitor _mailMonitor;
        
        public MailManagerBL(IConfigReader configReader, IMailMonitor mailMonitor)
        {
            _configReader = configReader;
            _mailMonitor = mailMonitor;
        }
        public async Task StartJob(CancellationToken token)
        {
            Console.WriteLine("\nНачинаю работу ...");

            #region Создаем xml для тестов
            //IConfigWriter configWriter = new ConfigWriter();
            //configWriter.WriteConfig();
            #endregion

            //считывание конфигурации
            Console.WriteLine("\nСчитывание конфигурации ...");
            List<ConfigEntity> configEntityList = await Task.Run(() => _configReader.ReadConfig()); // _configReader.ReadConfig();            
            //Thread.Sleep(3000);

            if (token.IsCancellationRequested)
                return;

            if (configEntityList == null || configEntityList.Count == 0)
                throw new ApplicationException("Файл конфигурации пуст!");

            //запускаем мониторинг почты            
            Console.WriteLine("\nЗапускаем мониторинг почты ...");
            await Task.Run(()=> _mailMonitor.StartMonitor(configEntityList));

            
        }

        public void StopJob()
        {
            //останавливаем мониторинг почты            
            Console.WriteLine("\nОстанавливаем мониторинг почты ...");
            _mailMonitor.StopMonitor();
        }
    }
}
