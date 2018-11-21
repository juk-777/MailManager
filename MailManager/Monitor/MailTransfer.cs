using System.Collections.Generic;

namespace MailManager.Monitor
{
    public class MailTransfer
    {        
        public List<MailEntity> MailEntities { get; set; }
        public List<string> Uids { get; set; }

        public MailTransfer()
        {
            MailEntities =  new List<MailEntity>();
            Uids = new List<string>();
        }
    }
}
