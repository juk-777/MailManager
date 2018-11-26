using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using OpenPop.Mime;
using OpenPop.Pop3;
using System.Linq;

namespace MailManager.Monitor
{
    public class OpenPopProvider : IMailProvider
    {
        public MailTransfer GetAllMessages(ConfigEntity configEntity)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(configEntity.Mail, configEntity.Port, useSsl: true);
                client.Authenticate(configEntity.Login, configEntity.Password);

                int messageCount = client.GetMessageCount();
                List<Message> allMessagesOpenPop = new List<Message>(messageCount);

                var mailTransfer = new MailTransfer();

                for (int i = messageCount; i > 0; i--)
                {
                    allMessagesOpenPop.Add(client.GetMessage(i));
                    mailTransfer.Uids.Add(client.GetMessageUid(i));
                }

                mailTransfer.MailEntities = ConvertMassageToMailEntity(allMessagesOpenPop);
                return mailTransfer;
            }                        
        }

        public MailTransfer GetUnseenMessages(ConfigEntity configEntity, List<string> seenUids)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(configEntity.Mail, configEntity.Port, useSsl: true);
                client.Authenticate(configEntity.Login, configEntity.Password);

                var mailTransfer = new MailTransfer();
                List<string> uids = client.GetMessageUids();
                List<Message> newMessages = new List<Message>();

                for (int i = 0; i < uids.Count; i++)
                {
                    string currentUidOnServer = uids[i];
                    if (!seenUids.Contains(currentUidOnServer))
                    {
                        Message unseenMessage = client.GetMessage(i + 1);
                        newMessages.Add(unseenMessage);
                        mailTransfer.Uids.Add(currentUidOnServer);
                    }
                }

                mailTransfer.MailEntities = ConvertMassageToMailEntity(newMessages);
                return mailTransfer;
            }
        }

        private List<MailEntity> ConvertMassageToMailEntity(List<Message> messages)
        {
            List<MailEntity> retMessages = new List<MailEntity>(messages.Count);            

            foreach (Message mes in messages)
            {
                var mailEntity = new MailEntity();

                mailEntity.To = (from s in mes.Headers.To select s.MailAddress).ToList();
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
    }
}
