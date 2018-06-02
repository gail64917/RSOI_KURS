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
        public IActionResult Index()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Index", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        [Route("Users")]
        public IActionResult GetUsers()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            try
            {
                string result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Users, "/api/users", null, null).Result;
                List<User> objectToView = JsonConvert.DeserializeObject<List<User>>(result);
                StatisticSender.SendStatistic("Home", DateTime.Now, "GetUsers", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
                return View(objectToView);
            }
            catch
            {
                return View("Error", "Service of Users unavailable");
            }
        }

        [Route("Orders")]
        public IActionResult GetOrders()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            string result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Orders, "/api/OrderItems", null, null).Result;
            try
            {
                List<OrderItems> objectToView = JsonConvert.DeserializeObject<List<OrderItems>>(result);
                StatisticSender.SendStatistic("Home", DateTime.Now, "GetOrders", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
                return View(objectToView);
            }
            catch
            {
                return View("Error", "Service of Orders Unavailable");
            }
        }

        [Route("Products")]
        public IActionResult GetProducts()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            try
            {
                string result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/Products", null, null).Result;
                List<Product> objectToView = JsonConvert.DeserializeObject<List<Product>>(result);
                StatisticSender.SendStatistic("Home", DateTime.Now, "GetOrders", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
                return View(objectToView);
            }
            catch
            {
                return View("Error", "Service of Orders unavailable");
            }
        }

        [Route("Products/Create")]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [Route("Products/Edite/{id?}")]
        public IActionResult EditeProduct(int? id)
        {
            string result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/Products/"+id, null, null).Result;
            Product objectToView = JsonConvert.DeserializeObject<Product>(result);
            return View(objectToView);
        }

        [HttpPost]
        [Route("Products/Edite/{id?}")]
        public IActionResult EditeProduct(int? id, [Bind("ID,ProductCategory", "ProductName", "Cost", "CountInBase")] Product prod)
        {
            try
            {
                var json = new JObject();
                json.Add("ID", prod.ID);
                json.Add("ProductCategory", prod.ProductCategory);
                json.Add("ProductName", prod.ProductName);
                json.Add("Cost", prod.Cost);
                json.Add("CountInBase", prod.CountInBase);
                string result = QueryClient.SendQueryToService(HttpMethod.Put, RabbitDLL.Linker.Products, "/api/Products/"+id, null, json).Result;
                Product objectToView = JsonConvert.DeserializeObject<Product>(result);

                result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/Products", null, null).Result;
                List<Product> objectToViewList = JsonConvert.DeserializeObject<List<Product>>(result);
                return View("GetProducts", objectToViewList);
            }
            catch
            {
                return View("Error", "Service of Products unavailable or cannot add new Product");
            }
        }

        [HttpPost]
        [Route("Products/Create")]
        public IActionResult CreateProduct([Bind("ProductCategory","ProductName","Cost","CountInBase")] Product prod)
        {
            try
            {
                var json = new JObject();
                json.Add("ProductCategory", prod.ProductCategory);
                json.Add("ProductName", prod.ProductName);
                json.Add("Cost", prod.Cost);
                json.Add("CountInBase", prod.CountInBase);
                string result = QueryClient.SendQueryToService(HttpMethod.Post, RabbitDLL.Linker.Products, "/api/Products", null, json).Result;
                Product objectToView = JsonConvert.DeserializeObject<Product>(result);

                result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/Products", null, null).Result;
                List<Product> objectToViewList = JsonConvert.DeserializeObject<List<Product>>(result);
                return View("GetProducts", objectToViewList);
            }
            catch
            {
                return View("Error", "Service of Products unavailable or cannot add new Product");
            }
        }

        [Route("Products/Delete/{id?}")]
        public IActionResult DeleteProduct(int? id)
        {
            try
            {
                string result = QueryClient.SendQueryToService(HttpMethod.Delete, RabbitDLL.Linker.Products, "/api/Products/"+id, null, null).Result;
                Product objectToView = JsonConvert.DeserializeObject<Product>(result);

                result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/Products", null, null).Result;
                List<Product> objectToViewList = JsonConvert.DeserializeObject<List<Product>>(result);
                return View("GetProducts", objectToViewList);
            }
            catch
            {
                return View("Error", "Service of Products unavailable or cannot add new Product");
            }
        }

        [Route("AllItems")]
        [Authorize(Policy = "Admin")]
        public IActionResult AllItems()
        {
            try
            {
                string result = QueryClient.SendQueryToService(HttpMethod.Get, RabbitDLL.Linker.Products, "/api/products/full/cortege", null, null).Result;
                FullView objectToView = JsonConvert.DeserializeObject<FullView>(result);
                string user = HttpContext.Session.GetString("Login");
                user = user != null ? user : "";
                StatisticSender.SendStatistic("Home", DateTime.Now, "All items", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
                return View(objectToView);
            }
            catch
            {
                return View("Error", "Service of Products unavailable");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "About", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Contacts", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        public IActionResult Error()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Error", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("LogOut")]
        public IActionResult LogOut()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "LogOut", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            HttpContext.Session.SetString("Token", "");
            HttpContext.Session.SetString("Login", "");
            return RedirectToAction(nameof(Index));
        }

        [Route("Registration")]
        public IActionResult Registration()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Registration Start", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
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

            string usr = HttpContext.Session.GetString("Login");
            usr = usr != null ? usr : "";

            try
            {
                var result = await QueryClient.SendQueryToService(HttpMethod.Post, "http://localhost:54196", "/api/Users", null, values);
                User resultUser = JsonConvert.DeserializeObject<User>(result);

                StatisticSender.SendStatistic("Home", DateTime.Now, "Registration", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, usr);
                return RedirectToAction("Authorisation");
            }
            catch
            {
                StatisticSender.SendStatistic("Home", DateTime.Now, "Registration", Request.HttpContext.Connection.RemoteIpAddress.ToString(), false, usr);
                return View("Error", "Cannot create this User. Try again later or input another Data");
            }
        }

        [Route("Authorisation")]
        public IActionResult Authorisation()
        {
            string user = HttpContext.Session.GetString("Login");
            user = user != null ? user : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Authorisation Start", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user);
            return View();
        }

        [Route("Authorisation")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authoristation([Bind("Login, Password")] User user)
        {
            string user1 = HttpContext.Session.GetString("Login");
            user1 = user1 != null ? user1 : "";
            StatisticSender.SendStatistic("Home", DateTime.Now, "Authorisation Ends", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, user1);
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
                    return View("Error", "User input incorrect or Service unavailable");
                }
            }
            else
            {
                return View("Error", "User input incorrect or Service unavailable");
            }
        }

        [Route("Submit")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SubmitAnket(Anket anket)
        {
            OrderItems order = new OrderItems(anket.Adress, anket.Name + ":" + anket.Email, anket.Money, anket.Adress);
            int BookCost = 2000;
            int GameCost = 3000;

            if(anket.Book != 0 && anket.Computer != 0)
            {
                decimal partBook = anket.Book / (anket.Book + anket.Computer);
                decimal partComp = anket.Computer / (anket.Book + anket.Computer);
                int countAllItems = Convert.ToInt32(Convert.ToInt32(anket.Money) / (partBook * BookCost + partComp * GameCost));

                int CountBook = Convert.ToInt32(partBook * countAllItems);
                int CountGame = Convert.ToInt32(partComp * countAllItems);
                string OrderItems = "Harry Potter=" + CountBook + "Witcher 3=" + CountGame;
                order.productsInBox = OrderItems;
                //api / OrderItems

                var values = new JObject();
                values.Add("Adress", order.Adress);
                values.Add("Cost", order.Cost);
                values.Add("DeliveryInfo", order.DeliveryInfo);
                values.Add("ProductsInBox", order.productsInBox);
                values.Add("UserInfo", order.UserInfo);

                string usr = HttpContext.Session.GetString("Login");
                usr = usr != null ? usr : "";

                try
                {
                    var result = await QueryClient.SendQueryToService(HttpMethod.Post, Linker.Orders, "/api/OrderItems", null, values);
                    OrderItems resultUser = JsonConvert.DeserializeObject<OrderItems>(result);

                    StatisticSender.SendStatistic("Home", DateTime.Now, "Registration", Request.HttpContext.Connection.RemoteIpAddress.ToString(), true, usr);
                    return View("SuccessSub", resultUser);
                }
                catch
                {
                    StatisticSender.SendStatistic("Home", DateTime.Now, "Registration", Request.HttpContext.Connection.RemoteIpAddress.ToString(), false, usr);
                    return View("Error", "Cannot create order");
                }
            }
            else
            {
                return View("Index");
            }
        }
    }
}
