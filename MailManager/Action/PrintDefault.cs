using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class PrintDefault : IPrint
    {
        private StringBuilder MailResult { get; set; }        

        public bool PrintTo(MailEntity message)
        {            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nPrint ...");
            Console.ForegroundColor = ConsoleColor.Gray;            

            MailResult = new StringBuilder();
            MailResult.Append("To:      " + MailMonitor.GetMailTo(message));
            MailResult.AppendLine();
            MailResult.Append("From:    " + message.From);
            MailResult.AppendLine();
            MailResult.Append("Subject: " + message.Subject);
            MailResult.AppendLine();
            MailResult.Append("Body:    " + message.Body);

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintPageHandler;

            printDoc.Print();
            Console.WriteLine("Письмо распечатано");
            return true;
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(MailResult.ToString(), new Font("Arial", 14), Brushes.Black, 0, 0);
        }
    }
}
