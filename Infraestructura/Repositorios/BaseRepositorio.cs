using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios
{
    public abstract class BaseRepositorio<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        protected BaseRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Set<T>().AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
        }
    }
}
