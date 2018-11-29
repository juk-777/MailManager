using System.Collections.Generic;
using System.Text;
using MailManager.Config;
using System.IO;
using System;

namespace MailManager.Monitor
{
    class TxtReadSeenUids : IReadSeenUids
    {
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
