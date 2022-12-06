using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
    where TEntity : class, IEntity, new()
    {
        protected readonly DbContext Context;
        
        public EfEntityRepositoryBase(DbContext context)
        {
            this.Context = context;
        }

        public  IQueryable<TEntity> QueryGetList(Expression<Func<TEntity, bool>> filter = null)
        {

            return filter == null
                ? Context.Set<TEntity>()
                : Context.Set<TEntity>().Where(filter);

        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
          
               await Context.Set<TEntity>().AddAsync(entity);
               return entity;
           
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate)
        {
            
                return await Context.Set<TEntity>().AnyAsync(predicate);
            
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().CountAsync(predicate);
        }
        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => 
            { 
                    Context.Set<TEntity>().Remove(entity); 
            });
        }
        public async Task DeleteAllAsync(IList<TEntity> entityList)
        {
            await Task.Run(() =>
            {
                Context.Set<TEntity>().RemoveRange(entityList);
            });
        }
        
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (predicate !=null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.SingleOrDefaultAsync();
           
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {

                    query =  query.Include(includeProperty);
                }
            }
           
            return await query.ToListAsync();
        }


        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                Context.Set<TEntity>().Update(entity);
            });
            return entity;
        }
    }
}
