﻿using System;
using MailManager.BusinessLogic;
using MailManager.Config;
using System.Threading;
using MailManager.Action;
using MailManager.Monitor;
using Unity;
using Unity.Injection;
using MailAction = MailManager.Action.MailAction;
using System.IO;

namespace MailManager
{
    static class Program
    {
        static void Main()
        {
            var container = new UnityContainer();

            Console.WriteLine("Добро пожаловать в MailManager ...");
            Console.WriteLine("\nВведите '1' для того, чтобы считать конфигурацию из файла App.config.");
            Console.WriteLine("Введите '0' для того, чтобы считать конфигурацию из другого файла.");
            int answer = Convert.ToInt32(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    Console.WriteLine("Считываю конфигурацию из файла App.config");
                    container.RegisterType<IConfigStream, AppConfigStream>();
                    break;
                case 0:                    
                    var configPath = GetFile();
                    string fileExtension = Path.GetExtension(configPath);
                    if (fileExtension == ".xml")
                        container.RegisterType<IConfigStream, XmlConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
                    else throw new ApplicationException("Не поддерживаемый формат файла!");
                    break;
                default:                    
                    Console.WriteLine("Ошибочный ввод. \nСчитываю конфигурацию из файла App.config");
                    container.RegisterType<IConfigStream, AppConfigStream>();
                    break;
            }

            #region RegisterTypes

            container.RegisterType<IConfigReader, ConfigReader>();
            container.RegisterType<IConfigVerify, ConfigVerify>();
            container.RegisterType<IMailMonitor, MailMonitor>();
            container.RegisterType<IMailProvider, OpenPopProvider>();
            container.RegisterType<IMailAction, MailAction>();
            container.RegisterType<ISaveSeenUids, TxtSaveSeenUids>();
            container.RegisterType<IReadSeenUids, TxtReadSeenUids>();
            container.RegisterType<IMailSender, SmtpSender>();
            container.RegisterType<IMailCopy, CopyToFolder>();
            container.RegisterType<INotify, ConsoleNotify>();
            container.RegisterType<IPrint, PrintDefault>();            
            container.RegisterType<IMailBusinessLogic, MailBusinessLogic>();

            var businessLogic = container.Resolve<IMailBusinessLogic>();

            #endregion

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nДля запуска работы нажмите Enter");
                Console.WriteLine("Для завершения работы нажмите Enter");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadLine();

                businessLogic.StartJob(token);

                Console.WriteLine();
                Console.ReadLine();
                cts.Cancel();

                Console.WriteLine("\nЗавершение работы ...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                container.Dispose();
                businessLogic.Dispose();
            }            

            Console.WriteLine("\nДо скорой встречи в MailManager ...");
            Console.ReadLine();
        }

        private static string GetFile()
        {            
            string configPath = Path.Combine(@"Files", @"MailManagerSettings.xml");
            Console.WriteLine("--------------------");
            Console.WriteLine($"Введите '1' для того, чтобы считать конфигурацию из предварительно подготовленного XML файла \n{configPath}");
            Console.WriteLine("\nЛибо введите любой другой путь к файлу конфигурации.");
            configPath = Console.ReadLine();            

            switch (configPath)
            {
                case "1":
                    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files", @"MailManagerSettings.xml");
                    break;
                default:
                    if (!string.IsNullOrEmpty(configPath))
                    {                        
                        FileInfo fileInf = new FileInfo(configPath);
                        if (!fileInf.Exists)
                            throw new ApplicationException("Ошибка при указании пути к файлу конфигурации!");                        
                    }
                    else throw new ApplicationException("Ошибка при указании пути к файлу конфигурации!");                    

                    break;
            }

            return configPath;
        }
    }
}
