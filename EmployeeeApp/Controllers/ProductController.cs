using ClosedXML.Excel;
using EmployeeeApp.Data;
using EmployeeeApp.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Cell = iText.Layout.Element.Cell;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Table = iText.Layout.Element.Table;

namespace EmployeeeApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductData _productData;
        private readonly ClientData _clientData;

        public ProductController()
        {
            _productData = new ProductData();
            _clientData = new ClientData();
        }

        public IActionResult Index()
        {
            var product = _productData.GetProducts();
            return View(product);
        }

        public IActionResult Order(int id)
        {
            var cli = _clientData.GetById(id);
            if (cli == null) return NotFound("Client not found.");

            var order = new Order
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
            if (order != null && _productData.PlaceOrder(order))
                return RedirectToAction("Index", "Client");

            return RedirectToAction("Orders", new { Id = order.ClientId });
        }

        public IActionResult Orders(int Id)
        {
            ViewBag.ClientId = Id;
            var order = _productData.OrderForId(Id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            if (_productData.RemoveOrder(orderId))
                return RedirectToAction("Index", "Client");
            return NotFound();
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            var list = _productData.GetProducts();
            return View(list);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        public async Task<IActionResult> PrintInvoice(int OrderId)
        {
            var orderl = await _productData.GetOrder(OrderId);
            var products = _productData.GetProducts();

            if (orderl == null) return NotFound();

            var address = _clientData.GetClientDetailsById(orderl.order.AddressId);

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            var bf = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var rf = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.jpg");
            if (System.IO.File.Exists(imgPath))
            {
                var imgData = ImageDataFactory.Create(imgPath);
                var logo = new iText.Layout.Element.Image(imgData)
                    .SetWidth(100f).SetHeight(100f)
                    .SetHorizontalAlignment(HorizontalAlignment.LEFT);
                doc.Add(logo);
            }

            doc.Add(new Paragraph($"Invoice for Order # {OrderId}").SetFont(bf).SetFontSize(14));
            doc.Add(new Paragraph($"Order for {orderl.Name}").SetFont(rf));
            doc.Add(new Paragraph($"Delivery Address: {address?.CityName ?? "N/A"}, {address?.StateName ?? "N/A"}, {address?.Pincode ?? "N/A"}").SetFont(rf));

            var tp = new Paragraph()
                .Add(new Text("Total price of the Order "))
                .Add(new Text($"{orderl.order.TotalPrice} Rs/- ").SetFont(bf));
            doc.Add(tp);

            doc.Add(new Paragraph("Order Summary").SetFont(rf));
            doc.Add(new Paragraph(" "));

            var tbl = new Table(new float[] { 1, 1, 2, 1, 1 });
            new[] { "Item ID", "Product ID", "Product Name", "Qty", "Total" }
                .ToList()
                .ForEach(h => tbl.AddHeaderCell(new Cell().Add(new Paragraph(h).SetFont(bf))));

            foreach (var item in orderl.order.OrderItems)
            {
                var prod = products.FirstOrDefault(p => p.ProductId == item.productId);
                var pn = prod?.ProductName ?? "Unknown Product";
                tbl.AddCell(new Cell().Add(new Paragraph(item.OrderItemId.ToString())));
                tbl.AddCell(new Cell().Add(new Paragraph(item.productId.ToString())));
                tbl.AddCell(new Cell().Add(new Paragraph(pn)));
                tbl.AddCell(new Cell().Add(new Paragraph(item.quantity.ToString())));
                tbl.AddCell(new Cell().Add(new Paragraph(item.TotalPrice.ToString())));
            }

            doc.Add(tbl);
            doc.Add(new Paragraph(" ").SetFontSize(10));
            doc.Add(new Paragraph("Great things are not done by impulse, but by a series of small things brought together — just like your order. Thank you for being part of our journey.").SetFontSize(10));
            doc.Close();

            return File(ms.ToArray(), "application/pdf", $"Invoice_{OrderId}.pdf");
        }

        [HttpGet]
        public IActionResult ExportInvoiceToWord(int OrderId)
        {
            var orderl = _productData.GetOrder(OrderId).Result;
            var products = _productData.GetProducts();
            var address = _clientData.GetClientDetailsById(orderl.order.AddressId);

            using var stream = new MemoryStream();
            using var doc = DocX.Create(stream);

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.jpg");
            if (System.IO.File.Exists(imgPath))
            {
                var img = doc.AddImage(imgPath);
                var pic = img.CreatePicture(100, 100);
                doc.InsertParagraph().AppendPicture(pic);
            }

            doc.InsertParagraph($"Invoice for Order # {OrderId}").Bold().FontSize(14);
            doc.InsertParagraph($"Order for {orderl.Name}");
            doc.InsertParagraph($"Delivery Address: {address?.CityName ?? "N/A"}, {address?.StateName ?? "N/A"}, {address?.Pincode ?? "N/A"}");
            doc.InsertParagraph($"Total price: {orderl.order.TotalPrice} Rs/-").Bold();
            doc.InsertParagraph("Order Summary").Bold();

            var tbl = doc.AddTable(orderl.order.OrderItems.Count + 1, 5);
            tbl.Design = TableDesign.LightListAccent1;
            new[] { "Item ID", "Product ID", "Product Name", "Qty", "Total" }
                .Select((h, i) => (h, i))
                .ToList()
                .ForEach(x => tbl.Rows[0].Cells[x.i].Paragraphs[0].Append(x.h).Bold());

            for (int i = 0; i < orderl.order.OrderItems.Count; i++)
            {
                var item = orderl.order.OrderItems[i];
                var prod = products.FirstOrDefault(p => p.ProductId == item.productId);
                var pn = prod?.ProductName ?? "Unknown Product";
                tbl.Rows[i + 1].Cells[0].Paragraphs[0].Append(item.OrderItemId.ToString());
                tbl.Rows[i + 1].Cells[1].Paragraphs[0].Append(item.productId.ToString());
                tbl.Rows[i + 1].Cells[2].Paragraphs[0].Append(pn);
                tbl.Rows[i + 1].Cells[3].Paragraphs[0].Append(item.quantity.ToString());
                tbl.Rows[i + 1].Cells[4].Paragraphs[0].Append(item.TotalPrice.ToString());
            }

            doc.InsertTable(tbl);
            doc.Save();

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Invoice_{OrderId}.docx");
        }

        [HttpGet]
        public IActionResult ExportClientsToExcel()
        {
            var allClients = _clientData.GetALL();
            if (allClients == null || !allClients.Any())
                return NotFound("No clients found to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ClientList");

          
            worksheet.Cell(1, 1).Value = "Client ID";
            worksheet.Cell(1, 2).Value = "Client Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Role";
            worksheet.Cell(1, 5).Value = "Number of Orders";

            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var client in allClients)
            {
                var orders = _productData.OrderForId(client.Id);
                worksheet.Cell(row, 1).Value = client.Id;
                worksheet.Cell(row, 2).Value = client.Name;
                worksheet.Cell(row, 3).Value = client.Email ?? "-";
                worksheet.Cell(row, 4).Value = client.Role ?? "-";
                worksheet.Cell(row, 5).Value = orders?.Count ?? 0;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"ClientList_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportInvoiceToExcel(int OrderId)
        {
            var orderl = await _productData.GetOrder(OrderId);
            if (orderl == null)
                return NotFound("Order not found.");

            var products = _productData.GetProducts();
            var address = _clientData.GetClientDetailsById(orderl.order.AddressId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Invoice");
            worksheet.ShowGridLines = false;
            int currentRow = 1;

          
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.jpg");
            if (System.IO.File.Exists(imagePath))
            {
                var img = worksheet.AddPicture(imagePath)
                    .MoveTo(worksheet.Cell(currentRow, 1))
                    .WithSize(100, 100);
                currentRow = 6;
            }

            worksheet.Cell(currentRow, 1).Value = $"Invoice for Order # {OrderId}";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;

            worksheet.Cell(currentRow + 1, 1).Value = $"Client Name: {orderl.Name}";
            worksheet.Cell(currentRow + 2, 1).Value = $"Address: {address?.CityName ?? "N/A"}, {address?.StateName ?? "N/A"}, {address?.Pincode ?? "N/A"}";
            worksheet.Cell(currentRow + 3, 1).Value = $"Total Price: {orderl.order.TotalPrice} Rs/-";

            int tableStart = currentRow + 5;

            
            worksheet.Cell(tableStart, 1).Value = "Item ID";
            worksheet.Cell(tableStart, 2).Value = "Product ID";
            worksheet.Cell(tableStart, 3).Value = "Product Name";
            worksheet.Cell(tableStart, 4).Value = "Qty";
            worksheet.Cell(tableStart, 5).Value = "Total";

            
            var headerRange = worksheet.Range(tableStart, 1, tableStart, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = tableStart + 1;
            foreach (var item in orderl.order.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.ProductId == item.productId);
                string productName = product?.ProductName ?? "Unknown Product";

                worksheet.Cell(row, 1).Value = item.OrderItemId;
                worksheet.Cell(row, 2).Value = item.productId;
                worksheet.Cell(row, 3).Value = productName;
                worksheet.Cell(row, 4).Value = item.quantity;
                worksheet.Cell(row, 5).Value = item.TotalPrice;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"Invoice_{OrderId}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int orderId)
        {
            var orderlist = await _productData.GetOrder(orderId);
            if (orderlist == null) return NotFound($"Order not found for preview: {orderId}");

            var address = _clientData.GetClientDetailsById(orderlist.order.AddressId);
            orderlist.City = address?.CityName ?? "N/A";
            orderlist.State = address?.StateName ?? "N/A";
            orderlist.PinCode = address?.Pincode ?? "N/A";

            return View(orderlist);
        }
    }
}
