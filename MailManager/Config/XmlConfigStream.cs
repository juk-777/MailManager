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
        public List<ConfigEntity> ReadStream()
        {            
            if (SettingsPath == null)
                throw new ApplicationException("Не указан путь к файлу конфигурации!");

            Console.WriteLine($"Файл: XML. Путь: {SettingsPath} ...");

            List<ConfigEntity> configEntityList = new List<ConfigEntity>();

            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<ConfigEntity>));

            // десериализация
            try
            {
                using (FileStream fs = new FileStream(SettingsPath, FileMode.OpenOrCreate))
                {
                    configEntityList = (List<ConfigEntity>)formatter.Deserialize(fs);
                    Console.WriteLine("Объекты десериализованы");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            return configEntityList;
        }
    }
}
