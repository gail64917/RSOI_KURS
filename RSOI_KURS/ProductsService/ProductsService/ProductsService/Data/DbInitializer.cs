using RabbitDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Data
{
    public class DbInitializer
    {
        public static void Initialize(ProductContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var products = new Product[]
            {
            new Product{ ProductCategory = "Book", ProductName = "Harry Potter", Cost = 2000, CountInBase = 1},
            new Product{ ProductCategory = "ComputerItem" , ProductName = "The Witcher 3", Cost = 2500, CountInBase = 2}
            };
            foreach (Product p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();


            var categories = new Category[]
            {
                new Category{ CategoryName = "For Erudite"},
                new Category{ CategoryName = "For Men"},
                new Category{ CategoryName = "For Woman" }
            };
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            var product_categories = new Product_Category[]
            {
                new Product_Category() { CategoryID = 1, ProductID = 1, Strength = 10 },
                new Product_Category() { CategoryID = 2, ProductID = 1, Strength = 7 },
                new Product_Category() { CategoryID = 3, ProductID = 1, Strength = 7 },
                new Product_Category() { CategoryID = 1, ProductID = 2, Strength = 3 },
                new Product_Category() { CategoryID = 2, ProductID = 2, Strength = 5 },
                new Product_Category() { CategoryID = 3, ProductID = 2, Strength = 3 }
            };

            foreach (Product_Category pc in product_categories)
            {
                context.Product_Categories.Add(pc);
            }
            context.SaveChanges();
        }
    }
}
