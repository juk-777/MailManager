﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MailManager.Config
{
    public class ConfigWriter : IConfigWriter
    {
        public void WriteConfig()
        {
            #region Разные способы массив/коллекция
            //IList<MailAction> mailActionsList = new List<MailAction>();
            //mailActionsList.Add(new MailAction("Notify", " yes"));
            //mailActionsList.Add(new MailAction("Print", " yes"));
            //configEntity.MailActionsList = mailActionsList;
            //configEntity.ActionMas = new string[,] { { "Notify", " yes" }, { "Print", " yes" } };
            #endregion

            // объект для сериализации
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

            //ConfigEntity[] configEntityMas = new ConfigEntity[] { configEntity1, configEntity2 };

            IList<ConfigEntity> configEntityList = new List<ConfigEntity>();
            configEntityList.Add(configEntity1);
            configEntityList.Add(configEntity2);

            Console.WriteLine("Объекты созданы");

            // передаем в конструктор тип класса
            //XmlSerializer formatter = new XmlSerializer(typeof(ConfigEntity));
            //XmlSerializer formatter = new XmlSerializer(typeof(ConfigEntity[]));
            XmlSerializer formatter = new XmlSerializer(typeof(List<ConfigEntity>));

            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(@"Files\MailManagerSettings.xml", FileMode.OpenOrCreate))
            {
                //formatter.Serialize(fs, configEntity1);
                formatter.Serialize(fs, configEntityList);

                Console.WriteLine("Объекты сериализованы");
            }
        }
    }
}