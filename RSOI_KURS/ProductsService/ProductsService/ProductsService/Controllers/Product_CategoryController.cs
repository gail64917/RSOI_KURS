using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsService.Data;
using RabbitDLL;

namespace ProductsService.Controllers
{
    [Produces("application/json")]
    [Route("api/Product_Category")]
    public class Product_CategoryController : Controller
    {
        //public class FullModel
        //{
        //    public string ProductCategory;
        //    public string ProductName;      
        //    public string CategoryName;
        //    public string 
        //}

        private readonly ProductContext _context;

        public Product_CategoryController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/Product_Category
        [HttpGet]
        public IEnumerable<Product_Category> GetProduct_Categories()
        {
            return _context.Product_Categories;
        }

        // GET: api/Product_Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct_Category([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product_Category = await _context.Product_Categories.SingleOrDefaultAsync(m => m.ID == id);

            if (product_Category == null)
            {
                return NotFound();
            }

            return Ok(product_Category);
        }

        // PUT: api/Product_Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct_Category([FromRoute] int id, [FromBody] Product_Category product_Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product_Category.ID)
            {
                return BadRequest();
            }

            _context.Entry(product_Category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Product_CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Product_Category
        [HttpPost]
        public async Task<IActionResult> PostProduct_Category([FromBody] Product_Category product_Category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Product_Categories.Add(product_Category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct_Category", new { id = product_Category.ID }, product_Category);
        }

        // DELETE: api/Product_Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct_Category([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product_Category = await _context.Product_Categories.SingleOrDefaultAsync(m => m.ID == id);
            if (product_Category == null)
            {
                return NotFound();
            }

            _context.Product_Categories.Remove(product_Category);
            await _context.SaveChangesAsync();

            return Ok(product_Category);
        }

        private bool Product_CategoryExists(int id)
        {
            return _context.Product_Categories.Any(e => e.ID == id);
        }
    }
}