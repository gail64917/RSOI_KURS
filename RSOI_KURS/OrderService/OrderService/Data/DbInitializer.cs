using RabbitDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data
{
    public class DbInitializer
    {
        public static void Initialize(OrderContext context)
        {
            context.Database.EnsureCreated();

            if (context.OrderItems.Any())
            {
                return;   // DB has been seeded
            }
            Product first = new Product { ProductCategory = "Book", ProductName = "Harry Potter", Cost = 2000, CountInBase = 1 };
            Product second = new Product { ProductCategory = "ComputerItem", ProductName = "The Witcher 3", Cost = 2500, CountInBase = 2 };

            Dictionary<Product, int> firstOrder = new Dictionary<Product, int>();
            firstOrder.Add(first, 2);

            string firstString = string.Join(";", firstOrder.Select(x => x.Key.ProductName + "=" + x.Value).ToArray());


            Dictionary<Product, int> secondOrder = new Dictionary<Product, int>();
            secondOrder.Add(first, 1);
            secondOrder.Add(second, 1);

            string secondString = string.Join(";", secondOrder.Select(x => x.Key.ProductName + "=" + x.Value).ToArray());

            var orders = new OrderItems[]
            {

                new OrderItems("Geroyev Panfilovcev, 14/53", "Vladimir", 3000, "Во второй половине дня, номер для связи +7925ххххххх") { productsInBox = firstString },
                new OrderItems("Фрязино, пр. Мира, 10/40", "Наталия", 2300, "Перед доставкой позвонить, номер для связи +7926ххххххх") { productsInBox = secondString }
            };

            foreach (OrderItems p in orders)
            {
                context.OrderItems.Add(p);
            }
            context.SaveChanges();
        }
    }
}
