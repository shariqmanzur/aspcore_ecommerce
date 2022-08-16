using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeProperties = null)
        {
            IQueryable<T> values = _dbSet;
            if(expression != null)
            {
                values = values.Where(expression);
            }
            if(includeProperties != null)
            {
                foreach(var item in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(item);
                }
            }
            return values.ToList();
        }

        public T GetT(Expression<Func<T, bool>> expression, string? includeProperties)
        {
            IQueryable<T> values = _dbSet;
            values = values.Where(expression);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(item);
                }
            }
            return values.FirstOrDefault();
        }
    }
}
