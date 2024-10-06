using GroceryStore.Models;
using GroceryStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GroceryStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _repository.CreateAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ID }, createdProduct);
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _repository.GetAllAsync(pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            var response = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = products
            };

            return Ok(response);
        }

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Продукт с ID {id} не найден." });

            return Ok(product);
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ID)
                return BadRequest(new { Message = "ID в URL не совпадает с ID в теле запроса." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repository.UpdateAsync(product);
            if (!result)
                return NotFound(new { Message = $"Продукт с ID {id} не найден." });

            return NoContent();
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound(new { Message = $"Продукт с ID {id} не найден." });

            return NoContent();
        }

        // GET: api/Products/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportProductsToCsv()
        {
            var products = await _repository.GetAllAsync(1, int.MaxValue); // Получить все продукты

            var csv = new StringBuilder();
            csv.AppendLine("ID,Название продукта,Описание,Цена,Дата добавления");

            foreach (var product in products)
            {
                csv.AppendLine($"{product.ID},{EscapeCsv(product.Name)},{EscapeCsv(product.Description)},{product.Price},{product.CreatedAt}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "products.csv");
        }

        private string EscapeCsv(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
