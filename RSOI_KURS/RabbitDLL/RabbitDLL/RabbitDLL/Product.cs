using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitDLL
{
    public class Product
    {
        public int ID { get; set; }
        public string ProductCategory { get; set; }
        public string ProductName { get; set; }
        public float Cost { get; set; }
        public int CountInBase { get; set; }

        public ICollection<Product_Category> BoundedWith { get; set; }

        public Product(string productCategory, string productName, float cost, int countInBase)
        {
            this.ProductCategory = productCategory;
            this.ProductName = productName;
            this.Cost = cost;
            this.CountInBase = countInBase;
        }

        public Product() { }
    }
}
