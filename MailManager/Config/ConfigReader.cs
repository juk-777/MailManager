using System.Collections.Generic;

namespace MailManager.Config
{
    public class ConfigReader : IConfigReader
    {
        private readonly IConfigStream _configStream;

        public ConfigReader(IConfigStream configStream)
        {
            _configStream = configStream;
        }
        public List<ConfigEntity> ReadConfig()
        {
            List<ConfigEntity> configEntityList = _configStream.ReadStream();
            return configEntityList;
        }
    }
}
