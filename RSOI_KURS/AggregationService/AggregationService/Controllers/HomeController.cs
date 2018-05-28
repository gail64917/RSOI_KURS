using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AggregationService.Models;
using RabbitDLL;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Threading;

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
        [Authorize(Policy = "Admin")]
        public IActionResult AllItems()
        {
            string result = QueryClient.SendQueryToService(HttpMethod.Get, "http://localhost:51229", "/api/products/full", null, null).Result;
            List<Product> prd = JsonConvert.DeserializeObject<List<Product>>(result);

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

        [Route("LogOut")]
        public IActionResult LogOut()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Default", DateTime.Now, "LogOut", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            HttpContext.Session.SetString("Token", "");
            HttpContext.Session.SetString("Login", "");
            return RedirectToAction(nameof(Index));
        }

        [Route("Registration")]
        public IActionResult Registration()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Default", DateTime.Now, "Registration Start", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        [Route("Registration")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([Bind("Login, Password")] User user)
        {
            var values = new JObject();
            values.Add("Login", user.Login);
            values.Add("Password", user.Password);
            values.Add("Role", "User");

            try
            {
                var result = await QueryClient.SendQueryToService(HttpMethod.Post, "http://localhost:54196", "/api/Users", null, values);
                User resultUser = JsonConvert.DeserializeObject<User>(result);
                return RedirectToAction("Authorisation");
            }
            catch
            {
                return View("Error", "Cannot create this User");
            }
        }

        [Route("Authorisation")]
        public IActionResult Authorisation()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Default", DateTime.Now, "Authorisation Start", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        [Route("Authorisation")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authoristation([Bind("Login, Password")] User user)
        {
            string user1 = HttpContext.Session.GetString("Login");
            user1 = user1 != null ? user1 : "";
            StatisticSender.SendStatistic("Default", DateTime.Now, "Authorisation Ends", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user1);
            return await privateAuth(user);
        }

        public async Task<IActionResult> privateAuth(User realUser)
        {
            if(realUser != null)
            {
                realUser = TokenController.CreateToken(realUser);
                if (realUser != null)
                {
                    HttpContext.Session.SetString("Token", realUser.LastToken);
                    HttpContext.Session.SetString("Login", realUser.Login);
                    return View("Index");
                }
                else
                {
                    return View("Error", "User input incorrect");
                }
            }
            else
            {
                return View("Error", "User input incorrect");
            }
        }
    }
}
