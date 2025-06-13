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
    }
}
