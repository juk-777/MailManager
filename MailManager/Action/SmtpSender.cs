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
        public bool SendTo(ConfigEntity configEntity, MailEntity message, string mailActionValue)
        {
            if (string.IsNullOrEmpty(mailActionValue))
            {
                Console.WriteLine("Почтовый ящик не указан!");
                return false;                                
            }
                        
            Console.ForegroundColor = ConsoleColor.Cyan;            
            Console.WriteLine($"\nForward: {mailActionValue}");
            Console.ForegroundColor = ConsoleColor.Gray;

            StringBuilder mailFrom = new StringBuilder();
            mailFrom.Append(configEntity.Mail);
            mailFrom.Replace("pop.", "");

            MailAddress from = new MailAddress(configEntity.Login + "@" + mailFrom);
            MailAddress to = new MailAddress(mailActionValue);
            MailMessage m = new MailMessage(from, to) {Subject = message.Subject, Body = message.Body.ToString()};
            SmtpClient smtp = new SmtpClient("smtp." + mailFrom, 587)
            {
                Credentials = new NetworkCredential(configEntity.Login, configEntity.Password), EnableSsl = true
            };
            smtp.SendMailAsync(m);

            Console.WriteLine("Письмо отправлено");
            return true;
        }        
    }
}
