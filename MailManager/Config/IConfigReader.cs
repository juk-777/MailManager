using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigReader
    {
        List<ConfigEntity> ReadConfig();        
    }
}
