using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MailManager.Config
{
    public class XmlConfigStream : IConfigStream
    {
        public string SettingsPath { get; set; }

        public XmlConfigStream(string setPath)
        {
            SettingsPath = setPath;
        }
        public IList<ConfigEntity> ReadStream()
        {            
            if (SettingsPath == null)
                throw new ApplicationException("Не указан путь к файлу конфигурации!");

            Console.WriteLine($"Файл: XML. Путь: {SettingsPath} ...");

            IList<ConfigEntity> configEntityList = new List<ConfigEntity>();

            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<ConfigEntity>));

            // десериализация
            using (FileStream fs = new FileStream(@"Files\MailManagerSettings.xml", FileMode.OpenOrCreate))
            {
                //Person[] newpeople = (Person[])formatter.Deserialize(fs);
                configEntityList = (List<ConfigEntity>)formatter.Deserialize(fs);

                Console.WriteLine("Объекты десериализованы");

                //foreach (ConfigEntity configEntity in configEntityList)
                //{
                //    Console.WriteLine($"Mail: {configEntity.Mail}   Login: {configEntity.Login}   Password: {configEntity.Password}");
                //}
            }

            return configEntityList;
        }
    }
}
