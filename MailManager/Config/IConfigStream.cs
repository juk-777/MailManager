using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigStream
    {
        IList<ConfigEntity> ReadStream();
    }
}
