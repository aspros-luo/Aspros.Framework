using Framework.Infrastructure.Interfaces.Core.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Framework.Infrastructure.Interfaces.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _dbContext;
       
        public UnitOfWork(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IDbContext DbContext => _dbContext;
        public DatabaseFacade Database => _dbContext.Database;
        public IDbConnection Connection => _dbContext.Database.GetDbConnection();
        //public IDbTransaction Transaction { get; set; }
        public IDbContextTransaction BeginTransaction(IDbContextTransaction dbContextTransaction = null)
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            if (dbContextTransaction != null)
            {
                return DbContextTransaction = dbContextTransaction;
            }

            return DbContextTransaction = Database.BeginTransaction();
        }

        public IDbContextTransaction DbContextTransaction { get; set; }

        public async Task<int> ExecuteSqlCommandAsync(string sql, CancellationToken cancellationToken = new CancellationToken(),
            params object[] parameters)
        {
            return await _dbContext.Database.ExecuteSqlCommandAsync(sql, cancellationToken, parameters);
        }

        public async Task<bool> RegisterNew<TEntity>(TEntity entity) where TEntity : class
        {

            _dbContext.Set<TEntity>().Add(entity);
            if (DbContextTransaction != null)
                return await _dbContext.SaveChangesAsync() > 0;
            return true;
        }
        public async Task<bool> RegisterDirty<TEntity>(TEntity entity) where TEntity : class
        {
            //            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<TEntity>().Update(entity);
            if (DbContextTransaction != null)
                return await _dbContext.SaveChangesAsync() > 0;
            return true;
        }
     
        public async Task<bool> RegisterDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (DbContextTransaction != null)
                return await _dbContext.SaveChangesAsync() > 0;
            return true;
        }
        public async Task<bool> RegisterRangeDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            if (DbContextTransaction != null)
                return await _dbContext.SaveChangesAsync() > 0;
            return true;
        }
        public async Task<bool> CommitAsync()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
            if (DbContextTransaction == null)
            {
                var result = await _dbContext.SaveChangesAsync() > 0;
                Connection.Close();
                Connection.Dispose();
                return result;
            }
            DbContextTransaction.Commit();
            DbContextTransaction = null;
            Connection.Close();
            Connection.Dispose();
            return true;
        }
        public void Rollback()
        {
            DbContextTransaction?.Rollback();
        }
    }
}
