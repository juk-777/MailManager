using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using System.IO;
using System;

namespace MailManager.Monitor
{
    public class TxtSeenUids : ISeenUidsManager
    {
        public bool Write(ConfigEntity configEntity, List<string> seenUids, bool addWrite)
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

        public List<string> Read(ConfigEntity configEntity)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Files", configEntity.Mail + "_" + configEntity.Login + "_SeenUids" + ".txt");
            List<string> seenUids = new List<string>();

            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    seenUids.Add(line);
                }
            }

            return seenUids;
        }
    }
}
