using System;
using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public class CopyToFolder : IMailCopy
    {
        private readonly ICopyWork _copyWork;

        public CopyToFolder(ICopyWork copyWork)
        {
            _copyWork = copyWork;
        }

        public bool CopyTo(ConfigEntity configEntity, MailEntity message, int rowNumber)
        {
            Console.WriteLine($"CopyToFolder: {configEntity.MailActions[rowNumber].ActTypeValue}");

            return _copyWork.DoWork(configEntity, message, rowNumber);
        }
    }
}
