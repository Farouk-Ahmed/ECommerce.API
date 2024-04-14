using ECommerce.API.Models;

namespace ECommerce.API.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        public IShoppingRepository shoppingRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
