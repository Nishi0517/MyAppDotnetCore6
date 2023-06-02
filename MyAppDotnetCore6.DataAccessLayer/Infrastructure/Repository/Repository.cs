using Microsoft.EntityFrameworkCore;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppWebCore6.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAppDotnetCore6.DataAccessLayer.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly MyAppDbContext _context;
        private DbSet<T> _dbSet;
        public Repository(MyAppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate=null, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
            //return _dbSet.ToList();
        }

        public T GetT(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(predicate);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();
        }
    }

}
