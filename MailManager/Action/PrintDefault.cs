using System.Text;
using MailManager.Config;
using MailManager.Monitor;
using System;
using System.Drawing.Printing;
using System.Drawing;

namespace MailManager.Action
{
    public class PrintDefault : IPrint
    {
        public StringBuilder MailResult { get; set; }        

        public bool PrintTo(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nPrint ...");
            Console.ForegroundColor = color;            

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
