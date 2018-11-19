using System.Text;
using MailManager.Config;
using MailManager.Monitor;
using System;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public class PrintDefault : IPrint
    {
        private readonly IPrintWork _printWork;

        public PrintDefault(IPrintWork printWork)
        {
            _printWork = printWork;
        }

        public async Task<bool> PrintToAsync(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            Console.WriteLine($"Print ...");

            StringBuilder mailResult = new StringBuilder();
            mailResult.Append("To:      " + MailMonitor.GetMailTo(message));
            mailResult.AppendLine();
            mailResult.Append("From:    " + message.From);
            mailResult.AppendLine();
            mailResult.Append("Subject: " + message.Subject);
            mailResult.AppendLine();
            mailResult.Append("Body:    " + message.Body);

            return await Task.Run(() => _printWork.DoWork(mailResult));
        }
    }
}
