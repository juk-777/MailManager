using System;
using System.Text;
using MailManager.Config;
using MailManager.Monitor;
using System.Net.Mail;
using System.Net;

namespace MailManager.Action
{
    public class SendWork : ISendWork
    {
        public bool DoWork(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
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

            //await smtp.SendMailAsync(m);
        }
    }
}
