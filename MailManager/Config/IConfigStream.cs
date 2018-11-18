using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigStream
    {
        List<ConfigEntity> ReadStream();
    }
}
