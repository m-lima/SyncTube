using System.Threading.Tasks;

namespace SyncTube.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}