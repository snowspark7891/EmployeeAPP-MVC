using EmployeeeApp.Data;
using EmployeeeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeeApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly Data.ProductData _productData;

        public ProductController()
        {
            _productData = new ProductData();
        }
        public IActionResult Index()
        {
            List<Product> product = _productData.GetProducts();
           
            return View(product);
        }

        public IActionResult Order(int id) {
            Order order = new Order
            {
                ClientId = id
            };
            return View(order);
        }
    }
}
