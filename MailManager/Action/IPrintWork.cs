using System.Text;

namespace MailManager.Action
{
    public interface IPrintWork
    {
        bool DoWork(StringBuilder sb);
    }
}
