using System.Linq;
using Framework.Domain.Core.Interface;

namespace Framework.Domain.Core
{
    public interface IRepository<out TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        IQueryable<TAggregateRoot> GetAll();
    }
}
