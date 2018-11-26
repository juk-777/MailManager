using System.Collections.Generic;

namespace MailManager.Config
{
    public interface IConfigVerify
    {
        bool VerifyConfig(List<ConfigEntity> configEntityList);
    }
}
