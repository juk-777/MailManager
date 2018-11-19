using System;
using System.Text;
using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class ConsoleNotify : INotify
    {
        public bool NotifyToAsync(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            Console.WriteLine($"Notify ...");

            StringBuilder mailTo = new StringBuilder();
            mailTo = MailMonitor.GetMailTo(message);

            Console.WriteLine();            
            Console.WriteLine($"To:      {mailTo}");
            Console.WriteLine($"From:    {message.From}");
            Console.WriteLine($"Data:    {message.DateSent}");
            Console.WriteLine($"Subject: {message.Subject}");
            Console.WriteLine($"Body:    {message.Body}");

            return true;
        }        
    }
}
