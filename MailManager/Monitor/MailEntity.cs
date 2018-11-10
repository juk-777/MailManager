using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace MailManager.Monitor
{
    public class MailEntity
    {
        public List<MailAddress> To { get; set; }
        public MailAddress From { get; set; }
        public string Subject { get; set; }
        public StringBuilder Body { get; set; }
        public DateTime DateSent { get; set; }

        public MailEntity()
        {
            To = new List<MailAddress>();
        }
    }
}
