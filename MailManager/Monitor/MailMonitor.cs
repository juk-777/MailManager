using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailManager.Config;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net.Mail;
using MailManager.Action;

namespace MailManager.Monitor
{
    public class MailMonitor : IMailMonitor
    {
        private readonly IMailProvider _mailProvider;
        private readonly IMailAction _mailAction;

        static object locker = new object();
        static private List<string> _seenUids = new List<string>();
        static private Timer _timer;
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
                //Console.WriteLine($"Mail: {configEntity.Mail} Port: { configEntity.Port} Login: {configEntity.Login} Password: {configEntity.Password}");

                Task.Run(() => StartMonitorTask(configEntity));
            }
        }

        public void StartMonitorTask(ConfigEntity configEntity)
        {
            lock (locker)
            {
                try
                {
                    _mailProvider.Initialize(configEntity);
                    _mailProvider.Connect();

                    //проходим первый раз по всем письмам и запоминаем их Uid
                    Console.WriteLine($"\nПервый проход по письмам {configEntity.Mail} ...");
                    List<string> seenUidsTemp = new List<string>();
                    seenUidsTemp = FirstAccessToMail(configEntity);
                    //_seenUids.Add(seenUidsTemp); // = FirstAccessToMail(); //configEntity);
                    foreach (string su in seenUidsTemp)
                    {
                        _seenUids.Add(su);
                    }

                    _mailProvider.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    _mailProvider?.Dispose();
                }

                // устанавливаем метод обратного вызова
                TimerCallback tm = new TimerCallback(OtherAccessToMail);
                // создаем таймер
                _timer = new Timer(tm, configEntity, 0, 10000);
                _timers.Add(_timer);
            }
        }

        public void StopMonitor()
        {
            foreach (Timer timer in _timers)
            {
                timer.Dispose();
            }
        }

        public List<string> FirstAccessToMail(ConfigEntity configEntity)
        {
            lock (locker)
            {
                List<MailEntity> allMessages = new List<MailEntity>();
                List<string> allUids = new List<string>();

                //получаем все письма и их Uid
                _mailProvider.GetAllMessages(out allMessages, out allUids);

                //обработка писем
                ProcessingMail(allMessages, configEntity);

                return allUids;
            }

        }

        public void OtherAccessToMail(object obj)
        {
            //последующие проходы с определенным интервалом
            lock (locker)
            {
                ConfigEntity configEntity = (ConfigEntity)obj;

                try
                {
                    _mailProvider.Initialize(configEntity);
                    _mailProvider.Connect();

                    Console.WriteLine($"\nВторичные проходы по письмам {configEntity.Mail} ...");
                    List<MailEntity> allMessages = _mailProvider.GetUnseenMessages(_seenUids, out _seenUids);

                    _mailProvider.Disconnect();

                    //обработка писем
                    ProcessingMail(allMessages, configEntity);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    _mailProvider?.Dispose();
                }
            }
        }

        public void ProcessingMail(List<MailEntity> messages, ConfigEntity configEntity)
        {
            lock (locker)
            {
                foreach (MailEntity mes in messages)
                {
                    ////List<MessagePart> mpart = mes.FindAllAttachments(); // находим  ВСЕ приаттаченные файлы
                    //StringBuilder mailBody = new StringBuilder();
                    StringBuilder mailTo = new StringBuilder();

                    //mailBody = GetMailBody(mes);
                    mailTo = GetMailTo(mes);

                    #region getMailBoby

                    //// ищем первую плейнтекст версию в сообщении
                    //MessagePart mpPlain = mes.FindFirstPlainTextVersion();
                    //if (mpPlain != null)
                    //{
                    //    //Encoding enc = mpPlain.BodyEncoding;
                    //    //body = enc.GetString(mpPlain.Body); //  получаем текст сообщения
                    //    mailBody.Append(mes.FindFirstPlainTextVersion().GetBodyAsText());
                    //}

                    #endregion

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
                        //Console.WriteLine();
                        //Console.WriteLine($"To:      {mailTo}");
                        //Console.WriteLine($"From:    {mes.From}");
                        //Console.WriteLine($"Subject: {mes.Subject}");
                        //Console.WriteLine($"Body:    {mes.Body}");

                        //выполнить действия над письмом
                        DoMailAction(configEntity, mes);
                    }
                }
            }

        }

        public void DoMailAction(ConfigEntity configEntity, MailEntity message)
        {
            lock (locker)
            {
                for (int i = 0; i < configEntity.MailActions.Length; i++)
                {
                    switch (configEntity.MailActions[i].ActType)
                    {
                        case ActionType.Notify:
                            _mailAction.ActionNotify(configEntity, message, i);
                            break;

                        case ActionType.CopyTo:                            
                            _mailAction.ActionCopy(configEntity, message, i);
                            break;

                        case ActionType.Forward:
                            //асинхронная отправка письма
                            //_mailAction.ActionSendEmailAsync(configEntity, message, i).GetAwaiter();
                            _mailAction.ActionSendAsync(configEntity, message, i);
                            break;

                        case ActionType.Print:                            
                            _mailAction.ActionPrint(configEntity, message, i);
                            break;
                    }
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

        
    }
}
