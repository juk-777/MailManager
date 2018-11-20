using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailManager.Config;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Mail;
using MailManager.Action;
using System.IO;

namespace MailManager.Monitor
{
    public class MailMonitor : IMailMonitor
    {
        private readonly IMailProvider _mailProvider;
        private readonly IMailAction _mailAction;
                
        private Timer _timer;
        public List<Timer> _timers = new List<Timer>();

        public MailMonitor(IMailProvider mailProvider, IMailAction mailAction)
        {
            _mailProvider = mailProvider;
            _mailAction = mailAction;
        }

        public void StartMonitor(List<ConfigEntity> configEntityList)
        {
            foreach (ConfigEntity configEntity in configEntityList)
            {
                Task.Run(() => StartMonitorTask(configEntity));
            }
        }

        public void StartMonitorTask(ConfigEntity configEntity)
        {            
            try
            {
                //проходим первый раз по всем письмам и запоминаем их Uid                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nПервый проход по письмам {configEntity.Mail} ...");
                Console.ForegroundColor = ConsoleColor.Gray;
                
                List<string> seenUidsTemp = new List<string>();
                seenUidsTemp = FirstAccessToMail(configEntity);
                List<string> seenUids = new List<string>();
                foreach (string su in seenUidsTemp)
                {
                    seenUids.Add(su);                                              
                }

                WriteFileSeenUids(configEntity, seenUids, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(OtherAccessToMail);
            // создаем таймер
            _timer = new Timer(tm, configEntity, 0, 10000);
            _timers.Add(_timer);            
        }

        private void WriteFileSeenUids(ConfigEntity configEntity, List<string> seenUids, bool addWrite)
        {            
            string path = @"Files";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            string writePath = path + "\\" + configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt";
            //Console.WriteLine($"\nЗаписываю данные в файл: {writePath}");

            StringBuilder seenUidsStrBuild = new StringBuilder();
            foreach (string su in seenUids)
            {
                seenUidsStrBuild.Append(su);
                seenUidsStrBuild.AppendLine();
            }

            using (StreamWriter sw = new StreamWriter(writePath, addWrite, System.Text.Encoding.Default))
            {
                sw.WriteLineAsync(seenUidsStrBuild.ToString().Trim());
            }         
        }

        public void StopMonitor()
        {
            foreach (Timer timer in _timers)
            {
                timer.Dispose();
            }

            _mailProvider?.Dispose();
        }

        public List<string> FirstAccessToMail(ConfigEntity configEntity)
        {            
            List<MailEntity> allMessages = new List<MailEntity>();
            List<string> allUids = new List<string>();

            //получаем все письма и их Uid
            _mailProvider.GetAllMessages(configEntity, out allMessages, out allUids);

            //обработка писем
            ProcessingMail(allMessages, configEntity);

            return allUids;
        }

        public void OtherAccessToMail(object obj)
        {
            //последующие проходы с определенным интервалом            
            ConfigEntity configEntity = (ConfigEntity)obj;

            try
            {                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nВторичные проходы по письмам {configEntity.Mail} ...");
                Console.ForegroundColor = ConsoleColor.Gray;
                
                string path = @"Files";                    
                path += "\\" + configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt";
                List<string> seenUids = new List<string>();

                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        seenUids.Add(line);
                    }
                }

                List<MailEntity> allMessages = _mailProvider.GetUnseenMessages(configEntity, seenUids, out seenUids);

                if (seenUids != null && seenUids.Count != 0)
                    WriteFileSeenUids(configEntity, seenUids, true);                                

                //обработка писем
                if (allMessages != null && allMessages.Count != 0)
                    ProcessingMail(allMessages, configEntity);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }                            
        }

        public void ProcessingMail(List<MailEntity> messages, ConfigEntity configEntity)
        {
            //Console.WriteLine($"\nProcessingMail ...");
            foreach (MailEntity mes in messages)
            {                
                StringBuilder mailTo = new StringBuilder();
                mailTo = GetMailTo(mes);
                //List<MessagePart> mpart = mes.FindAllAttachments(); // находим  ВСЕ приаттаченные файлы

                Regex[] regexMas = new Regex[configEntity.IdentityMessages.Length];
                MatchCollection[] matchesMas = new MatchCollection[configEntity.IdentityMessages.Length];

                for (int i = 0; i < configEntity.IdentityMessages.Length; i++)
                {
                    switch (configEntity.IdentityMessages[i].IdType)
                    {
                        case IdentityType.To:
                            regexMas[i] = new Regex($@"\w*{configEntity.IdentityMessages[i].IdTypeValue}\w*", RegexOptions.IgnoreCase);
                            matchesMas[i] = regexMas[i].Matches(mailTo.ToString());
                            break;
                        case IdentityType.From:
                            regexMas[i] = new Regex($@"\w*{configEntity.IdentityMessages[i].IdTypeValue}\w*", RegexOptions.IgnoreCase);
                            matchesMas[i] = regexMas[i].Matches(mes.From.ToString());
                            break;
                        case IdentityType.Title:
                            regexMas[i] = new Regex($@"\w*{configEntity.IdentityMessages[i].IdTypeValue}\w*", RegexOptions.IgnoreCase);
                            matchesMas[i] = regexMas[i].Matches(mes.Subject);
                            break;
                        case IdentityType.Body:
                            regexMas[i] = new Regex($@"\w*{configEntity.IdentityMessages[i].IdTypeValue}\w*", RegexOptions.IgnoreCase);
                            matchesMas[i] = regexMas[i].Matches(mes.Body.ToString());
                            break;
                    }
                }

                int[] matchesKolMas = new int[configEntity.IdentityMessages.Length];
                for (int i = 0; i < configEntity.IdentityMessages.Length; i++)
                {
                    if (matchesMas[i].Count > 0) matchesKolMas[i] = 1;
                    else matchesKolMas[i] = 0;
                }

                int sumKol = 0;
                for (int i = 0; i < configEntity.IdentityMessages.Length; i++)
                {
                    sumKol += matchesKolMas[i];
                }

                if (sumKol == configEntity.IdentityMessages.Length)
                {
                    //выполнить действия над письмом
                    DoMailActionAsync(configEntity, mes);
                }
            }            
        }

        public async void DoMailActionAsync(ConfigEntity configEntity, MailEntity message)
        {            
            for (int i = 0; i < configEntity.MailActions.Length; i++)
            {
                switch (configEntity.MailActions[i].ActType)
                {
                    case ActionType.Notify:                        
                        await Task.Run(() => _mailAction.ActionNotify(configEntity, message, i));
                        break;

                    case ActionType.CopyTo:                            
                        await Task.Run(() => _mailAction.ActionCopy(configEntity, message, i));
                        break;

                    case ActionType.Forward:
                        await Task.Run(() => _mailAction.ActionSend(configEntity, message, i));
                        break;

                    case ActionType.Print:                            
                        await Task.Run(() => _mailAction.ActionPrint(configEntity, message, i));
                        break;
                }
            }            
        }

        internal static StringBuilder GetMailTo(MailEntity message)
        {
            StringBuilder mailTo = new StringBuilder();

            foreach (MailAddress to in message.To)
                mailTo.Append(to);

            return mailTo;
        }

        #region GetMailBody

        //private StringBuilder GetMailBody(Message message)
        //{
        //    StringBuilder mailBody = new StringBuilder();

        //    // ищем первую плейнтекст версию в сообщении
        //    MessagePart mpPlain = message.FindFirstPlainTextVersion();
        //    if (mpPlain != null)
        //    {
        //        //Encoding enc = mpPlain.BodyEncoding;
        //        //body = enc.GetString(mpPlain.Body); //  получаем текст сообщения
        //        mailBody.Append(message.FindFirstPlainTextVersion().GetBodyAsText());
        //    }

        //    return mailBody;
        //}

        #endregion

    }
}
