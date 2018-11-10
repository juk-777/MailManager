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
            var xmlSettingsPath = @"Files\MailManagerSettings.xml";

            Console.WriteLine("Добро пожаловать в MailManager ...");

            #region Регион 

            #endregion

            /// <summary>
            /// Комменты
            /// </summary>

            #region Инициализация без IoC

            //IMailManagerBL mmBL = new MailManagerBL(new ConfigReader(new XmlConfigStream(xmlSettingsPath)),
            //    new MailMonitor(new OpenPopProvider(),
            //        new Action.MailAction(new SmtpSender(new SendWork()), new CopyToFolder(new CopyWork()), new ConsoleNotify(), new PrintDefault(new PrintWork()))));

            #endregion

            #region Инициализация с IoC

            var container = new UnityContainer();
            container.RegisterType<IConfigReader, ConfigReader>();
            container.RegisterType<IConfigWriter, ConfigWriter>();
            container.RegisterType<IConfigStream, XmlConfigStream>(new InjectionConstructor(new InjectionParameter<string>(xmlSettingsPath)));
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
