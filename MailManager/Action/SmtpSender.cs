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

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;            
            Console.WriteLine($"\nForward: {configEntity.MailActions[rowNumber].ActTypeValue}");
            Console.ForegroundColor = color;

            StringBuilder mailFrom = new StringBuilder();
            mailFrom.Append(configEntity.Mail);
            mailFrom.Replace("pop.", "");

            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(configEntity.Login + "@" + mailFrom.ToString());
            // кому отправляем
            MailAddress to = new MailAddress(configEntity.MailActions[rowNumber].ActTypeValue);
            // создаем объект сообщения
            MailMessage m = new MailMessage(@from, to);
            // тема письма
            m.Subject = message.Subject;
            // текст письма
            m.Body = message.Body.ToString();
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp." + mailFrom.ToString(), 587);
            smtp = new SmtpClient("smtp." + mailFrom.ToString(), 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(configEntity.Login, configEntity.Password);
            smtp.EnableSsl = true;
            smtp.SendMailAsync(m);

            Console.WriteLine("Письмо отправлено");
            return true;
        }

        #region Send

        //public void Send(ConfigEntity configEntity, MailEntity message, int rowNumber)
        //{
        //    StringBuilder mailFrom = new StringBuilder();
        //    mailFrom.Append(configEntity.Mail);
        //    mailFrom.Replace("pop.", "");

        //    // отправитель - устанавливаем адрес и отображаемое в письме имя
        //    MailAddress from = new MailAddress(configEntity.Login + "@" + mailFrom.ToString());
        //    // кому отправляем
        //    MailAddress to = new MailAddress(configEntity.MailActions[rowNumber].ActTypeValue);
        //    // создаем объект сообщения
        //    MailMessage m = new MailMessage(from, to);
        //    // тема письма
        //    m.Subject = message.Subject;
        //    // текст письма
        //    m.Body = message.Body.ToString();
        //    // адрес smtp-сервера и порт, с которого будем отправлять письмо
        //    SmtpClient smtp = new SmtpClient("smtp." + mailFrom.ToString(), 587);
        //    // логин и пароль
        //    smtp.Credentials = new NetworkCredential(configEntity.Login, configEntity.Password);
        //    smtp.EnableSsl = true;
        //    smtp.Send(m);
        //    Console.WriteLine("Письмо отправлено");
        //}

        #endregion

    }
}
