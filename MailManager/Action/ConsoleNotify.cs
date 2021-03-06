﻿using System;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class ConsoleNotify : INotify
    {
        public bool NotifyTo(MailEntity message)
        {            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nNotify ...");
            Console.ForegroundColor = ConsoleColor.Gray;

            var mailTo = MailMonitor.GetMailTo(message);

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
