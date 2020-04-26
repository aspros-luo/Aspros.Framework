using Framework.Infrastructure.Interfaces.Core.Interface;
using System.Linq;
using Framework.Domain.Core.Interface;

namespace Framework.Domain.Core
{
    public abstract class BaseRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        public readonly IQueryable<TAggregateRoot> Entities;

        protected BaseRepository(IDbContext dbContext)
        {
            Entities = dbContext.Set<TAggregateRoot>();
        }

        public IQueryable<TAggregateRoot> GetAll()
        {
            return Entities;
        }

    }

}
