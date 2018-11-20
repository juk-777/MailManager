﻿using MailManager.Config;
using MailManager.Monitor;

namespace MailManager.Action
{
    public interface IMailSender
    {
        bool SendTo(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
