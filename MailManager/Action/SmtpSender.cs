﻿using System;
using System.Threading.Tasks;
using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class SmtpSender : IMailSender
    {
        private readonly ISendWork _sendWork;

        public SmtpSender(ISendWork sendWork)
        {
            _sendWork = sendWork;
        }

        public async Task<bool> SendAsync(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            if (string.IsNullOrEmpty(configEntity.MailActions[rowNumber].ActTypeValue))
            {
                Console.WriteLine("Почтовый ящик не указан!");
                return false;
                //throw new ApplicationException("Почтовый ящик не указан!");
                
            }

            Console.WriteLine($"Forward: {configEntity.MailActions[rowNumber].ActTypeValue}");

            return await Task.Run(() => _sendWork.DoWork(configEntity, message, rowNumber));
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
