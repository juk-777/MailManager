using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using System.IO;
using System;

namespace MailManager.Monitor
{
    public class TxtSaveSeenUids : ISaveSeenUids
    {
        public bool Save(ConfigEntity configEntity, List<string> seenUids, bool addWrite)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Files";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            string writePath = Path.Combine(path, configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt");

            StringBuilder seenUidsStrBuild = new StringBuilder();
            foreach (string su in seenUids)
            {
                seenUidsStrBuild.Append(su);
                seenUidsStrBuild.AppendLine();
            }

            using (StreamWriter sw = new StreamWriter(writePath, addWrite, Encoding.Default))
            {
                sw.WriteLineAsync(seenUidsStrBuild.ToString().Trim());
            }

            return true;
        }
    }
}
