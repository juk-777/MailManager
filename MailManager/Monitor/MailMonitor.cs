using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailManager.Config;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Mail;
using MailManager.Action;
using System.Linq;

namespace MailManager.Monitor
{
    public class MailMonitor : IMailMonitor
    {
        private readonly IMailProvider _mailProvider;
        private readonly IMailAction _mailAction;
        private readonly ISeenUidsManager _seenUidsManager;
        private readonly List<Timer> _timers = new List<Timer>();

        public MailMonitor(IMailProvider mailProvider, IMailAction mailAction, ISeenUidsManager seenUidsManager)
        {
            _mailProvider = mailProvider;
            _mailAction = mailAction;
            _seenUidsManager = seenUidsManager;
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nПервый проход по письмам {configEntity.Mail} ...");
            Console.ForegroundColor = ConsoleColor.Gray;

            var seenUidsTemp = FirstAccessToMail(configEntity);
            var seenUids = from s in seenUidsTemp select s;

            if (!_seenUidsManager.Write(configEntity, seenUids.ToList(), false))
                throw new ApplicationException("Ошибка при сохранении Uid прочитанных писем!");

            OtherAccessToMail(configEntity);

            TimerCallback tm = OtherAccessToMail;
            var timer = new Timer(tm, configEntity, 0, 5000);
            _timers.Add(timer);
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

                List<string> seenUids = _seenUidsManager.Read(configEntity);

                var mailTransfer = _mailProvider.GetUnseenMessages(configEntity, seenUids);

                if (mailTransfer.Uids != null && mailTransfer.Uids.Count != 0)
                {
                    if (!_seenUidsManager.Write(configEntity, mailTransfer.Uids, true))
                        throw new ApplicationException("Ошибка при сохранении Uid прочитанных писем!");
                }                                

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
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\nОшибочные данные! {configEntity.IdentityMessages[i].IdType} - {configEntity.IdentityMessages[i].IdTypeValue}");
                            Console.ForegroundColor = ConsoleColor.Gray;
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
                        await Task.Run(() => _mailAction.ActionNotify(message));
                        break;

                    case ActionType.CopyTo:
                        var i2 = i;
                        await Task.Run(() => _mailAction.ActionCopy(configEntity, message, configEntity.MailActions[i2].ActTypeValue));
                        break;

                    case ActionType.Forward:
                        var i3 = i;
                        await Task.Run(() => _mailAction.ActionSend(configEntity, message, configEntity.MailActions[i3].ActTypeValue));
                        break;

                    case ActionType.Print:
                        await Task.Run(() => _mailAction.ActionPrint(message));
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nОшибочные данные! {configEntity.MailActions[i].ActType} - {configEntity.MailActions[i].ActTypeValue}");
                        Console.ForegroundColor = ConsoleColor.Gray;
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
                    foreach (var timer in _timers)
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
