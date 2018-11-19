﻿using MailManager.Config;
using MailManager.Monitor;
using System.Threading.Tasks;

namespace MailManager.Action
{
    public interface IPrint
    {
        Task<bool> PrintToAsync(ConfigEntity configEntity, MailEntity message, int rowNumber);
    }
}
