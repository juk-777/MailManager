using System;
using System.Threading;
using System.Threading.Tasks;

namespace MailManager.BusinessLogic
{
    public interface IMailBusinessLogic : IDisposable
    {
        Task StartJob(CancellationToken token);
    }
}
