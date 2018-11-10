using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigReader
    {
        IList<ConfigEntity> ReadConfig();
    }
}
