using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MailManager.Config
{
    public class XmlConfigStream : IConfigStream
    {
        private string SettingsPath { get; }

        public XmlConfigStream(string setPath)
        {
            SettingsPath = setPath;
        }
        public List<ConfigEntity> ReadStream()
        {            
            if (SettingsPath == null)
                throw new ArgumentNullException(nameof(SettingsPath), "Не указан путь к файлу конфигурации!");

            List<ConfigEntity> configEntityList;
            XmlSerializer formatter = new XmlSerializer(typeof(List<ConfigEntity>));
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
