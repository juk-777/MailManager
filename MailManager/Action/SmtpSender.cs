using System;
using MailManager.Config;
using MailManager.Monitor;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace MailManager.Action
{
    public class SmtpSender : IMailSender
    {        
        public bool SendTo(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            if (string.IsNullOrEmpty(configEntity.MailActions[rowNumber].ActTypeValue))
            {
                Console.WriteLine("Почтовый ящик не указан!");
                return false;                                
            }
                        
            Console.ForegroundColor = ConsoleColor.Cyan;            
            Console.WriteLine($"\nForward: {configEntity.MailActions[rowNumber].ActTypeValue}");
            Console.ForegroundColor = ConsoleColor.Gray;

            StringBuilder mailFrom = new StringBuilder();
            mailFrom.Append(configEntity.Mail);
            mailFrom.Replace("pop.", "");

            MailAddress from = new MailAddress(configEntity.Login + "@" + mailFrom.ToString());
            MailAddress to = new MailAddress(configEntity.MailActions[rowNumber].ActTypeValue);
            MailMessage m = new MailMessage(@from, to);
            m.Subject = message.Subject;
            m.Body = message.Body.ToString();
            SmtpClient smtp = new SmtpClient("smtp." + mailFrom.ToString(), 587);
            smtp = new SmtpClient("smtp." + mailFrom.ToString(), 587);
            smtp.Credentials = new NetworkCredential(configEntity.Login, configEntity.Password);
            smtp.EnableSsl = true;
            smtp.SendMailAsync(m);

            Console.WriteLine("Письмо отправлено");
            return true;
        }        
    }
}
