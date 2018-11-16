using System;
using MailManager.BL;
using MailManager.Config;
using System.Threading;
using MailManager.Action;
using MailManager.Monitor;
using Unity;
using Unity.Injection;
using MailAction = MailManager.Action.MailAction;

namespace MailManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = @"Files\MailManagerSettings.xml";

            Console.WriteLine("Добро пожаловать в MailManager ...");

            Console.WriteLine("Введите '1' для того, чтобы считать конфигурацию из файла App.config.");
            Console.WriteLine("Введите '0' для того, чтобы считать конфигурацию из другого файла.");
            int answer = Convert.ToInt32(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    Console.WriteLine("Считываю конфигурацию из файла App.config");
                    break; // переход к case 5
                case 0:
                    Console.WriteLine($"Введите '1' для того, чтобы считать конфигурацию из предварительно подготовленного XML файла {configPath}");
                    Console.WriteLine("Либо введите любой другой путь к файлу конфигурации.");
                    configPath = Console.ReadLine();

                    switch (configPath)
                    {
                        case "1":
                            configPath = @"Files\MailManagerSettings.xml";
                            Console.WriteLine($"Файл: XML. Путь: {configPath} ...");
                            break;
                        default:
                            Console.WriteLine($"Файл: xxx. Путь: {configPath} ...");
                            break;
                    }

                    break;
                default:                    
                    Console.WriteLine("Ошибочный ввод. \nСчитываю конфигурацию из файла App.config");
                    break;
            }

            #region Регион 

            #endregion

            /// <summary>
            /// Комменты
            /// </summary>

            #region Инициализация без IoC

            //IMailManagerBL mmBL = new MailManagerBL(new ConfigReader(new XmlConfigStream(configPath)),
            //    new MailMonitor(new OpenPopProvider(),
            //        new Action.MailAction(new SmtpSender(new SendWork()), new CopyToFolder(new CopyWork()), new ConsoleNotify(), new PrintDefault(new PrintWork()))));

            #endregion

            #region Инициализация с IoC

            var container = new UnityContainer();
            container.RegisterType<IConfigReader, ConfigReader>();
            container.RegisterType<IConfigWriter, ConfigWriter>();
            container.RegisterType<IConfigStream, XmlConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
            container.RegisterType<IMailMonitor, MailMonitor>();
            container.RegisterType<IMailProvider, OpenPopProvider>();
            container.RegisterType<IMailAction, MailAction>();

            container.RegisterType<IMailSender, SmtpSender>();
            container.RegisterType<IMailCopy, CopyToFolder>();
            container.RegisterType<INotify, ConsoleNotify>();
            container.RegisterType<IPrint, PrintDefault>();

            container.RegisterType<ISendWork, SendWork>();
            container.RegisterType<ICopyWork, CopyWork>();
            container.RegisterType<IPrintWork, PrintWork>();

            //container.RegisterType<IMailManagerBL, MailManagerBL>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMailManagerBL, MailManagerBL>();

            var mmBL = container.Resolve<IMailManagerBL>();

            #endregion
            
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nДля запуска работы нажмите Enter");
                Console.WriteLine("Для завершения работы нажмите Enter");
                Console.ForegroundColor = color;
                Console.ReadLine();

                mmBL.StartJob(token);

                Console.WriteLine();
                Console.ReadLine();
                cts.Cancel();

                mmBL.StopJob();
                Console.WriteLine("\nЗавершение работы ...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
                        
            Console.WriteLine("\nДо скорой встречи в MailManager ...");
            Console.ReadLine();
        }
    }
}
