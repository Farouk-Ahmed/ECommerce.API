using ECommerce.API.Data;
using ECommerce.API.Interface;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        public IShoppingRepository _shoppingRepository;
        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public IShoppingRepository shoppingRepository  => _shoppingRepository ?? new ShoppingRepository(_context);



        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
