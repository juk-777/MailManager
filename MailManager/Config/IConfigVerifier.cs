using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigVerifier
    {
        bool Verify(List<ConfigEntity> configEntityList);
    }
}
