using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T:class,IEntity,new() 
    {
        //List<T> GetList(Expression<Func<T, bool>> filter = null);
        //T Get(Expression<Func<T, bool>> filter);
        //T Find(Expression<Func<T, bool>> filter);
        //T Add(T entity);
        //T Update(T entity);
        //void Delete(T entity);
        Task<T> AddAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteAllAsync(IList<T> entityList);
        IQueryable<T> QueryGetList(Expression<Func<T, bool>> filter = null);
        
    }
}
