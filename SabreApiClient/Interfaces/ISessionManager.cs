using System.Threading.Tasks;
using Domain.Models;

namespace SabreApiClient.Interfaces
{
    public interface ISessionManager
    {
        Task<Session> CreateSession(Credentials credentials, string methodName);
        Task<string> CloseSession(Session session);
    }
}
