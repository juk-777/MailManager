using System.Threading;
using System.Threading.Tasks;

namespace MailManager.BL
{
    public interface IMailManagerBL
    {
        Task StartJob(CancellationToken token);
        void StopJob();
    }
}
