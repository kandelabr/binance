using System.Threading.Tasks;

namespace Binance.Handlers
{
    /// <summary>
    /// Notification handler interface
    /// </summary>
    public interface INotificationHandler
    {
        Task SendEmail(string subject, string message);
    }
}
