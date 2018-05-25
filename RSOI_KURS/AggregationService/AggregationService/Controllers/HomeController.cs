using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AggregationService.Models;
using RabbitDLL;
using Newtonsoft.Json;

namespace AggregationService.Controllers
{
    public class HomeController : Controller
    {
        public class FullView
        {
            public HashSet<string> categoryCollection = new HashSet<string>();
            public List<ProductCortege> productCollection = new List<ProductCortege>();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("AllItems")]
        public IActionResult AllItems()
        {
            string result = QueryClient.SendQueryToService("http://localhost:51229", "/api/products/full", null).Result;
            List<Product> prd = JsonConvert.DeserializeObject<List<Product>>(result);

            //распарсить в 
            List<ProductCortege> lst = new List<ProductCortege>();
            HashSet<string> Categories = new HashSet<string>();
            foreach(Product product in prd)
            {
                ProductCortege cortege = new ProductCortege(product.ProductName, product.Cost, product.CountInBase);
                foreach(Product_Category bound in product.BoundedWith)
                {
                    cortege.CategoryParameters.Add(bound.Category.CategoryName, bound.Strength);
                    Categories.Add(bound.Category.CategoryName);
                }
                lst.Add(cortege);
            }

            FullView objectToView = new FullView() { categoryCollection = Categories, productCollection = lst };

            return View(objectToView);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
