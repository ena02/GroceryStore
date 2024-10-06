using GroceryStore.Models;

namespace GroceryStore.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task<Product> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
