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
        public void GetAllMessages(ConfigEntity configEntity, out List<MailEntity> allMessages, out List<string> allUids)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(configEntity.Mail, configEntity.Port, useSsl: true);
                client.Authenticate(configEntity.Login, configEntity.Password);

                int messageCount = client.GetMessageCount();
                List<Message> allMessagesOP = new List<Message>(messageCount);

                allMessages = new List<MailEntity>(messageCount);
                allUids = new List<string>(messageCount);

                for (int i = messageCount; i > 0; i--)
                {
                    allMessagesOP.Add(client.GetMessage(i));
                    allUids.Add(client.GetMessageUid(i));
                }

                allMessages = ConvertMassageToMailEntity(allMessagesOP);
            }                        
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
            MessagePart mpPlain = message.FindFirstPlainTextVersion();
            if (mpPlain != null)
            {
                mailBody.Append(message.FindFirstPlainTextVersion().GetBodyAsText());
            }

            return mailBody;
        }

        public List<MailEntity> GetUnseenMessages(ConfigEntity configEntity, List<string> seenUids, out List<string> seenUidsNew)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(configEntity.Mail, configEntity.Port, useSsl: true);
                client.Authenticate(configEntity.Login, configEntity.Password);

                List<string> uids = client.GetMessageUids();
                seenUidsNew = new List<string>();
                List<Message> newMessages = new List<Message>();

                for (int i = 0; i < uids.Count; i++)
                {
                    string currentUidOnServer = uids[i];
                    if (!seenUids.Contains(currentUidOnServer))
                    {
                        Message unseenMessage = client.GetMessage(i + 1);
                        newMessages.Add(unseenMessage);
                        seenUidsNew.Add(currentUidOnServer);
                    }
                }

                List<MailEntity> retMessages = ConvertMassageToMailEntity(newMessages);                
                return retMessages;
            }  
        }              
    }
}
