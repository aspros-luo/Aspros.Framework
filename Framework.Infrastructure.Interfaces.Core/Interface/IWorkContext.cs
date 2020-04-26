using System.Threading.Tasks;

namespace Framework.Infrastructure.Interfaces.Core.Interface
{
    public interface IWorkContext
    {
        Task<long> GetUserId();

        Task<string> GetUserName();

        Task<string> GetUserNick();

        Task<object> GetUserType();

        Task<T> Get<T>(string key);
    }
}
