﻿using System;
using System.Collections.Generic;

namespace MailManager.Config
{
    public class TxtConfigStream : IConfigStream
    {
        public string SettingsPath { get; set; }

        public TxtConfigStream(string setPath)
        {
            SettingsPath = setPath;
        }
        public IList<ConfigEntity> ReadStream()
        {
            if (SettingsPath == null)
                throw new ApplicationException("Не указан путь к файлу конфигурации!");
            
            Console.WriteLine($"Файл: TXT. Путь: {SettingsPath} ...");

            ConfigEntity configEntity1 = new ConfigEntity();
            configEntity1.MailActions = new MailAction[] { new MailAction { ActType = ActionType.Notify, ActTypeValue = "yes" }, new MailAction { ActType = ActionType.Forward, ActTypeValue = "juk_777@mail.ru" } };
            configEntity1.IdentityMessages = new IdentityMessage[] { new IdentityMessage { IdType = IdentityType.From, IdTypeValue = "juk_777@mail.ru" }, new IdentityMessage { IdType = IdentityType.Title, IdTypeValue = "тест" } };
            configEntity1.Mail = "pop.mail.ru";
            configEntity1.Port = 995;
            configEntity1.Login = "gus.guskovskij";
            configEntity1.Password = "11guskovskij11";

            ConfigEntity configEntity2 = new ConfigEntity();
            configEntity2.MailActions = new MailAction[] { new MailAction { ActType = ActionType.Notify, ActTypeValue = "yes" }, new MailAction { ActType = ActionType.Print, ActTypeValue = "yes" } };
            configEntity2.IdentityMessages = new IdentityMessage[] { new IdentityMessage { IdType = IdentityType.From, IdTypeValue = "juk_777@mail.ru" }, new IdentityMessage { IdType = IdentityType.Title, IdTypeValue = "тест" } };
            configEntity2.Mail = "pop.yandex.ru";
            configEntity2.Port = 995;
            configEntity2.Login = "tiras.school.2";
            configEntity2.Password = "2TiraS2";

            IList<ConfigEntity> configEntityList = new List<ConfigEntity>();
            configEntityList.Add(configEntity1);
            configEntityList.Add(configEntity2);

            return configEntityList;
        }
    }
}
