using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace MailManager.Action
{
    public class PrintWork : IPrintWork
    {
        public StringBuilder MailResult { get; set; }

        public bool DoWork(StringBuilder sb)
        {
            MailResult = sb;

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
