using System;
using System.Threading;
using System.Threading.Tasks;

namespace MailManager.BL
{
    public interface IMailManagerBL : IDisposable
    {
        Task StartJob(CancellationToken token);
    }
}
