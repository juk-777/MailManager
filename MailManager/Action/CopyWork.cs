using System;
using System.Text;
using MailManager.Config;
using MailManager.Monitor;
using System.IO;

namespace MailManager.Action
{
    public class CopyWork : ICopyWork
    {
        public bool DoWork(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            string path = @"Files";
            string subpath = configEntity.MailActions[rowNumber].ActTypeValue;
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            if (subpath == null || subpath == "") subpath = "";
            else
                dirInfo.CreateSubdirectory(subpath);

            string writePath = path + "\\" + subpath + "\\" + configEntity.Mail + " " + message.DateSent.ToString("d") + ".txt";

            StringBuilder mailResult = new StringBuilder();
            mailResult.Append("To:      " + MailMonitor.GetMailTo(message));
            mailResult.AppendLine();
            mailResult.Append("From:    " + message.From);
            mailResult.AppendLine();
            mailResult.Append("Subject: " + message.Subject);
            mailResult.AppendLine();
            mailResult.Append("Body:    " + message.Body);

            using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLineAsync(mailResult.ToString());
            }

            Console.WriteLine("Письмо скопировано");
            return true;
        }
    }
}
