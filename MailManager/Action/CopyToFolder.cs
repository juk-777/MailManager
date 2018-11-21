using System;
using MailManager.Config;
using MailManager.Monitor;
using System.IO;
using System.Text;

namespace MailManager.Action
{
    public class CopyToFolder : IMailCopy
    {        
        public bool CopyTo(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nCopyToFolder: {configEntity.MailActions[rowNumber].ActTypeValue}");
            Console.ForegroundColor = ConsoleColor.Gray;            

            string path = @"Files";
            string subpath = configEntity.MailActions[rowNumber].ActTypeValue;
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            if (string.IsNullOrEmpty(subpath)) subpath = "";
            else
                dirInfo.CreateSubdirectory(subpath);

            string writePath = Path.Combine(path, subpath, configEntity.Mail + " " + message.DateSent.ToString("d") + ".txt");

            StringBuilder mailResult = new StringBuilder();
            mailResult.Append("To:      " + MailMonitor.GetMailTo(message));
            mailResult.AppendLine();
            mailResult.Append("From:    " + message.From);
            mailResult.AppendLine();
            mailResult.Append("Subject: " + message.Subject);
            mailResult.AppendLine();
            mailResult.Append("Body:    " + message.Body);

            using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
            {
                sw.WriteLineAsync(mailResult.ToString());
            }

            Console.WriteLine("Письмо скопировано");
            return true;
        }
    }
}
