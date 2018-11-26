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
using System.Linq;

namespace MailManager.Monitor
{
    public class MailMonitor : IMailMonitor
    {
        private readonly IMailProvider _mailProvider;
        private readonly IMailAction _mailAction;
                
        private Timer _timer;
        public List<Timer> Timers = new List<Timer>();

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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nПервый проход по письмам {configEntity.Mail} ...");
                Console.ForegroundColor = ConsoleColor.Gray;

                var seenUidsTemp = FirstAccessToMail(configEntity);               

                var seenUids = from s in seenUidsTemp select s;
                WriteFileSeenUids(configEntity, seenUids.ToList(), false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            TimerCallback tm = OtherAccessToMail;
            _timer = new Timer(tm, configEntity, 0, 10000);
            Timers.Add(_timer);            
        }

        private void WriteFileSeenUids(ConfigEntity configEntity, List<string> seenUids, bool addWrite)
        {            
            string path = @"Files";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }            
            string writePath = Path.Combine(path, configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt");

            StringBuilder seenUidsStrBuild = new StringBuilder();
            foreach (string su in seenUids)
            {
                seenUidsStrBuild.Append(su);
                seenUidsStrBuild.AppendLine();
            }

            using (StreamWriter sw = new StreamWriter(writePath, addWrite, Encoding.Default))
            {
                sw.WriteLineAsync(seenUidsStrBuild.ToString().Trim());
            }         
        }

        private List<string> FirstAccessToMail(ConfigEntity configEntity)
        {            
            try
            {
                var mailTransfer = _mailProvider.GetAllMessages(configEntity);
                ProcessingMail(mailTransfer.MailEntities, configEntity);

                return mailTransfer.Uids;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void OtherAccessToMail(object obj)
        {                       
            ConfigEntity configEntity = (ConfigEntity)obj;

            try
            {                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nВторичные проходы по письмам {configEntity.Mail} ...");
                Console.ForegroundColor = ConsoleColor.Gray;
                                
                string path = Path.Combine(@"Files", configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt");
                List<string> seenUids = new List<string>();

                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        seenUids.Add(line);
                    }
                }

                var mailTransfer = _mailProvider.GetUnseenMessages(configEntity, seenUids);

                if (mailTransfer.Uids != null && mailTransfer.Uids.Count != 0)
                    WriteFileSeenUids(configEntity, mailTransfer.Uids, true);                                

                if (mailTransfer.MailEntities != null && mailTransfer.MailEntities.Count != 0)
                    ProcessingMail(mailTransfer.MailEntities, configEntity);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }                            
        }

        private void ProcessingMail(List<MailEntity> messages, ConfigEntity configEntity)
        {            
            foreach (MailEntity mes in messages)
            {
                var mailTo = GetMailTo(mes);

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
                    DoMailActionAsync(configEntity, mes);
                }
            }            
        }

        private async void DoMailActionAsync(ConfigEntity configEntity, MailEntity message)
        {            
            for (int i = 0; i < configEntity.MailActions.Length; i++)
            {
                switch (configEntity.MailActions[i].ActType)
                {
                    case ActionType.Notify:
                        var i1 = i;
                        await Task.Run(() => _mailAction.ActionNotify(configEntity, message, i1));
                        break;

                    case ActionType.CopyTo:
                        var i2 = i;
                        await Task.Run(() => _mailAction.ActionCopy(configEntity, message, i2));
                        break;

                    case ActionType.Forward:
                        var i3 = i;
                        await Task.Run(() => _mailAction.ActionSend(configEntity, message, i3));
                        break;

                    case ActionType.Print:
                        var i4 = i;
                        await Task.Run(() => _mailAction.ActionPrint(configEntity, message, i4));
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

        #region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (Timer timer in Timers)
                    {
                        timer.Dispose();
                    }                                     
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
