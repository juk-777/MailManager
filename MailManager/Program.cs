using System;
using MailManager.BL;
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
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            string configPath;

            Console.WriteLine("Добро пожаловать в MailManager ...");
            Console.WriteLine("Введите '1' для того, чтобы считать конфигурацию из файла App.config.");
            Console.WriteLine("Введите '0' для того, чтобы считать конфигурацию из другого файла.");
            int answer = Convert.ToInt32(Console.ReadLine());

            switch (answer)
            {
                case 1:
                    Console.WriteLine("Считываю конфигурацию из файла App.config");
                    configPath = @"App.config";
                    container.RegisterType<IConfigStream, AppConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
                    break;
                case 0:
                    string fileExtension;
                    GetFile(out configPath, out fileExtension);
                    if (fileExtension == "xml")
                        container.RegisterType<IConfigStream, XmlConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
                    else if (fileExtension == "txt")
                        container.RegisterType<IConfigStream, TxtConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
                    else throw new ApplicationException("Неподдерживаемый формат файла!");
                    break;
                default:                    
                    Console.WriteLine("Ошибочный ввод. \nСчитываю конфигурацию из файла App.config");
                    configPath = @"App.config";
                    container.RegisterType<IConfigStream, AppConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
                    break;
            }            

            #region Инициализация без IoC

            //IMailManagerBL mmBL = new MailManagerBL(new ConfigReader(new XmlConfigStream(configPath)),
            //    new MailMonitor(new OpenPopProvider(),
            //        new Action.MailAction(new SmtpSender(new SendWork()), new CopyToFolder(new CopyWork()), new ConsoleNotify(), new PrintDefault(new PrintWork()))));

            #endregion

            #region Инициализация с IoC
            
            container.RegisterType<IConfigReader, ConfigReader>();
            container.RegisterType<IConfigWriter, ConfigWriter>();
            //container.RegisterType<IConfigStream, XmlConfigStream>(new InjectionConstructor(new InjectionParameter<string>(configPath)));
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
            finally
            {
                container.Dispose();
            }            

            Console.WriteLine("\nДо скорой встречи в MailManager ...");
            Console.ReadLine();
        }

        private static void GetFile(out string configPath, out string fileExtension)
        {
            configPath = @"Files\MailManagerSettings.xml";
            Console.WriteLine("--------------------");
            Console.WriteLine($"Введите '1' для того, чтобы считать конфигурацию из предварительно подготовленного XML файла \n{configPath}");
            Console.WriteLine("\nЛибо введите любой другой путь к файлу конфигурации.");
            configPath = Console.ReadLine();            

            switch (configPath)
            {
                case "1":
                    configPath = @"Files\MailManagerSettings.xml";
                    fileExtension = "xml";
                    Console.WriteLine($"Файл: XML. Путь: {configPath}");
                    break;
                default:
                    if (!string.IsNullOrEmpty(configPath))
                    {                        
                        FileInfo fileInf = new FileInfo(configPath);
                        if (fileInf.Exists)
                        {
                            fileExtension = fileInf.Name.Substring(fileInf.Name.LastIndexOf(".", StringComparison.Ordinal) + 1);
                            Console.WriteLine($"Файл: {fileExtension}. Путь: {configPath}");
                        }
                        else throw new ApplicationException("Ошибка при указании пути к файлу конфигурации!");                        
                    }
                    else throw new ApplicationException("Ошибка при указании пути к файлу конфигурации!");                    

                    break;
            }            
        }
    }
}
