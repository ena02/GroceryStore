using GroceryStore.Data;
using GroceryStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .OrderBy(p => p.ID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ID);
            if (existingProduct == null)
                return false;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            // Обновление даты добавления, если необходимо
            // existingProduct.CreatedAt = product.CreatedAt;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
