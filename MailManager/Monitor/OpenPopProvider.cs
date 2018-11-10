using System;
using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using OpenPop.Mime;
using OpenPop.Pop3;
using OpenPop.Mime.Header;

namespace MailManager.Monitor
{
    public class OpenPopProvider : IMailProvider
    {
        private bool _disposed = false;
        public ConfigEntity ConfigEntity { get; set; }
        private Pop3Client _client;

        public void Initialize(ConfigEntity configEntity)
        {
            ConfigEntity = configEntity;
            _client = new Pop3Client();
        }

        public void Connect()
        {
            //Pop3Client client = new Pop3Client(); //_client = new Pop3Client();

            // Connect to the server
            _client.Connect(ConfigEntity.Mail, ConfigEntity.Port, useSsl: true);

            // Authenticate ourselves towards the server
            _client.Authenticate(ConfigEntity.Login, ConfigEntity.Password);

            //Console.WriteLine($"\nСоединение {_configEntity.Mail} : {_configEntity.Port} установлено");
        }

        public void Disconnect()
        {
            _client.Disconnect();
            //Console.WriteLine($"\nСоединение {_configEntity.Mail} : {_configEntity.Port} разорвано");
        }

        public void GetAllMessages(out List<MailEntity> allMessages, out List<string> allUids)
        {

            // Get the number of messages in the inbox
            int messageCount = _client.GetMessageCount();

            // We want to download all messages
            List<Message> allMessagesOP = new List<Message>(messageCount);

            allMessages = new List<MailEntity>(messageCount);
            allUids = new List<string>(messageCount);

            // Messages are numbered in the interval: [1, messageCount]
            // Ergo: message numbers are 1-based.
            // Most servers give the latest message the highest number
            for (int i = messageCount; i > 0; i--)
            {
                allMessagesOP.Add(_client.GetMessage(i));
                allUids.Add(_client.GetMessageUid(i));
            }

            allMessages = ConvertMassageToMailEntity(allMessagesOP);
        }

        private List<MailEntity> ConvertMassageToMailEntity(List<Message> messages)
        {
            List<MailEntity> retMessages = new List<MailEntity>(messages.Count);            

            foreach (Message mes in messages)
            {
                var mailEntity = new MailEntity();

                foreach (RfcMailAddress to in mes.Headers.To)
                    mailEntity.To.Add(to.MailAddress);

                mailEntity.From = mes.Headers.From.MailAddress;
                mailEntity.Subject = mes.Headers.Subject;
                mailEntity.DateSent = mes.Headers.DateSent;
                mailEntity.Body = GetMailBody(mes);

                retMessages.Add(mailEntity);
            }

            return retMessages;
        }

        private StringBuilder GetMailBody(Message message)
        {
            StringBuilder mailBody = new StringBuilder();

            // ищем первую плейнтекст версию в сообщении
            MessagePart mpPlain = message.FindFirstPlainTextVersion();
            if (mpPlain != null)
            {
                //Encoding enc = mpPlain.BodyEncoding;
                //body = enc.GetString(mpPlain.Body); //  получаем текст сообщения
                mailBody.Append(message.FindFirstPlainTextVersion().GetBodyAsText());
            }

            return mailBody;
        }

        //private StringBuilder GetMailTo(Message message)
        //{
        //    StringBuilder mailTo = new StringBuilder();

        //    foreach (RfcMailAddress to in message.Headers.To)
        //        mailTo.Append(to);

        //    return mailTo;
        //}

        public List<MailEntity> GetUnseenMessages(List<string> seenUids, out List<string> seenUidsNew)
        {
            // Fetch all the current uids seen
            List<string> uids = _client.GetMessageUids();

            seenUidsNew = seenUids;

            // Create a list we can return with all new messages
            List<Message> newMessages = new List<Message>();

            // All the new messages not seen by the POP3 client
            for (int i = 0; i < uids.Count; i++)
            {
                string currentUidOnServer = uids[i];
                if (!seenUids.Contains(currentUidOnServer))
                {
                    // We have not seen this message before.
                    // Download it and add this new uid to seen uids

                    // the uids list is in messageNumber order - meaning that the first
                    // uid in the list has messageNumber of 1, and the second has 
                    // messageNumber 2. Therefore we can fetch the message using
                    // i + 1 since messageNumber should be in range [1, messageCount]
                    Message unseenMessage = _client.GetMessage(i + 1);

                    // Add the message to the new messages
                    newMessages.Add(unseenMessage);

                    // Add the uid to the seen uids, as it has now been seen
                    seenUidsNew.Add(currentUidOnServer);
                }
            }

            // Return our new found messages
            List<MailEntity> retMessages = ConvertMassageToMailEntity(newMessages);
            return retMessages;
        }

        public void Dispose()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы
                }
                // освобождаем неуправляемые объекты
                _disposed = true;
            }
        }        

        ~OpenPopProvider()
        {
            Dispose(false);
        }

        #region Работа с библиотекой

        //using (Pop3Client client = new Pop3Client())
        //{
        //    // Connect to the server
        //    client.Connect("pop.mail.ru", 995, true);

        //    // Authenticate ourselves towards the server
        //    client.Authenticate("gus.guskovskij", "11guskovskij11");


        //    List<string> msgs = client.GetMessageUids();  //получаем список айдишников писем в почте



        //    for (int i = 1; i <= msgs.Count; i++) //организация цикла по сообщениям в почте

        //    {



        //        Message msg = client.GetMessage(i); // получаем сообщение
        //        List<MessagePart> mpart = msg.FindAllAttachments(); // находим  ВСЕ приаттаченные файлы
        //        string body = "";

        //        // ищем первую плейнтекст версию в сообщении

        //        MessagePart mpPlain = msg.FindFirstPlainTextVersion();
        //        StringBuilder builder = new StringBuilder();
        //        if (mpPlain != null)
        //        {
        //            Encoding enc = mpPlain.BodyEncoding;
        //            body = enc.GetString(mpPlain.Body); //  получаем текст сообщения
        //            builder.Append(msg.FindFirstPlainTextVersion().GetBodyAsText());
        //        }

        //        if (mpart.Count > 0) // если есть аттачменты то …

        //        {



        //            foreach (MessagePart attach in mpart)

        //            {



        //                // read data from attachment  . допустим у меня в аттачменте текст в ЮТФ8. получу его содержание

        //                string ticket = Encoding.UTF8.GetString(attach.Body);

        //                // что-то делаю с ним

        //            }

        //            Console.WriteLine("Mail with subject " + msg.Headers.Subject + " is ready!");

        //        }
        //    }



        //// Get the number of messages in the inbox
        //int messageCount = client.GetMessageCount();

        //// We want to download all messages
        //List<Message> allMessages = new List<Message>(messageCount);

        //// Messages are numbered in the interval: [1, messageCount]
        //// Ergo: message numbers are 1-based.
        //// Most servers give the latest message the highest number
        //for (int i = messageCount; i > 0; i--)
        //{
        //    allMessages.Add(client.GetMessage(i));
        //}

        //foreach (Message mes in allMessages)
        //{
        //    Console.WriteLine($"{mes.Headers.From}   {mes.Headers.Subject}   {mes.MessagePart.GetBodyAsText()} ");
        //}
        //Console.WriteLine();

        // Now return the fetched messages
        //return allMessages;
        //}

        #endregion
    }
}
