using EmployeeeApp.Data;
using EmployeeeApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X509;



namespace EmployeeeApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly Data.ProductData _productData;
        private readonly Data.ClientData _clientData;
        public ProductController()
        {
            _productData = new ProductData();
            _clientData = new ClientData();
        }
        public IActionResult Index()
        {
            List<Product> product = _productData.GetProducts();

            return View(product);
        }

        public IActionResult Order(int id)
        {
            ClientView cli = _clientData.GetById(id);
            Order order = new Order
            {
                ClientId = id,
                ClientName = cli.Name,
                OrderItems = new List<OrderItems>()
            };
            return View(order);
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            if(order != null)
            {
                if (_productData.PlaceOrder(order)){
                    return RedirectToAction("Index","Client");
                }
            }
            return View("Order", order.ClientId);
        }


        public IActionResult Orders(int Id)
        {
           
            ViewBag.ClientId = Id;

            List<Order> order = _productData.OrderForId(Id);
            if (order != null) {
                return View(order);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId) 
        {
            if (_productData.RemoveOrder(orderId))
            {
                return RedirectToAction("Index", "Client");
            }
            return NotFound();
        }


        [HttpGet]
        public IActionResult ProductList()
        {
            List<Product> list = _productData.GetProducts();
            return View(list);
        }

        public IActionResult PrintInvoice(int OrderId)
        {
           var orderl = _productData.GetOrder(OrderId);
            var products = _productData.GetProducts();
           
            if (orderl != null)
            {
                var address = _clientData.GetClientDetailsById(orderl.order.AddressId);
                using (var stream = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4);
                    PdfWriter.GetInstance(doc, stream);
                    doc.Open();
                    doc.Add(new Paragraph($"Invoice for Order # {OrderId}"));
                    doc.Add(new Paragraph($"Order for {orderl.Name}"));
                    doc.Add(new Paragraph($"Delivery Address {address.CityName},{address.StateName},{address.Pincode}"));
                    doc.Add(new Paragraph($"Total price Of the Order : {orderl.order.TotalPrice}"));
                    doc.Add(new Paragraph(" "));

                    PdfPTable table = new PdfPTable(5);
                    table.AddCell("Item ID");
                    table.AddCell("Product ID");
                    table.AddCell("Product Name");
                    table.AddCell("Qty");
                    table.AddCell("Total");

                    foreach (var item in orderl.order.OrderItems)
                    {
                        var product = products.FirstOrDefault(p => p.ProductId == item.productId);
                        string productName = product != null ? product.ProductName : "Unknown Product";

                        table.AddCell(item.OrderItemId.ToString());
                        table.AddCell(item.productId.ToString());
                        table.AddCell(productName); 
                        table.AddCell(item.quantity.ToString());
                        table.AddCell(item.TotalPrice.ToString());
                    }


                    doc.Add(table);

                    doc.Add(new Paragraph("Great things are not done by impulse, but by a series of small things brought together — just like your order. Thank you for being part of our journey."));
                    doc.Close();

                    byte[] pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Invoice_{OrderId}.pdf");
                }
            }
            else
            {
                return NotFound();
            }
        }



    }
}


