using ClosedXML.Excel; // Required for Excel functionality
using EmployeeeApp.Data;
using EmployeeeApp.Models; // Ensure this namespace is correct for all your models (ClientView, Order, OrderItems, Product, ClientAddress)
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System; // Required for DateTime
using System.Collections.Generic;
using System.IO;
using System.Linq; // Required for LINQ methods like .Any(), .FirstOrDefault()

namespace EmployeeeApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductData _productData; // Use direct type for clarity
        private readonly ClientData _clientData;   // Use direct type for clarity

        public ProductController()
        {
            _productData = new ProductData();
            _clientData = new ClientData();
        }

        // --- Existing Action Methods ---

        public IActionResult Index()
        {
            List<Product> product = _productData.GetProducts();
            return View(product);
        }

        public IActionResult Order(int id)
        {
            ClientView cli = _clientData.GetById(id);
            if (cli == null)
            {
                // Handle case where client is not found
                return NotFound("Client not found.");
            }

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
            if (order != null)
            {
                if (_productData.PlaceOrder(order))
                {
                    // Redirect to the client list after a successful order
                    return RedirectToAction("Index", "Client");
                }
            }
            // If order placement fails, redirect back to the orders list for that client
            // Ensure order.ClientId is available if PlaceOrder failed.
            return RedirectToAction("Orders", new { Id = order.ClientId });
        }

        public IActionResult Orders(int Id)
        {
            ViewBag.ClientId = Id; // Pass ClientId to the view
            List<Order> order = _productData.OrderForId(Id);
            if (order != null)
            {
                return View(order);
            }
            return NotFound(); // Return 404 if no orders found for the ID
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            // You might want to get the ClientId before removing the order
            // to redirect back to that client's order list.
            // For now, it redirects to the general Client Index as per your original code.
            if (_productData.RemoveOrder(orderId))
            {
                return RedirectToAction("Index", "Client");
            }
            return NotFound(); // Return 404 if order not found or cancellation fails
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            List<Product> list = _productData.GetProducts();
            return View(list);
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        public IActionResult PrintInvoice(int OrderId)
        {
            var orderl = _productData.GetOrder(OrderId);
            var products = _productData.GetProducts();

            if (orderl != null)
            {
                // Assuming orderl.order.AddressId is of type int?
                var address = _clientData.GetClientDetailsById(orderl.order.AddressId);

                using (var stream = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(stream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    // --- Font Definitions ---
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // --- Logo ---
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.jpg");
                    if (System.IO.File.Exists(imagePath))
                    {
                        ImageData imageData = ImageDataFactory.Create(imagePath);
                        Image logo = new Image(imageData);
                        logo.SetWidth(100f).SetHeight(100f);
                        logo.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                        document.Add(logo);
                    }

                    // --- Invoice Header ---
                    document.Add(new Paragraph($"Invoice for Order # {OrderId}")
                                         .SetFont(boldFont)
                                         .SetFontSize(14));
                    document.Add(new Paragraph($"Order for {orderl.Name}")
                                         .SetFont(regularFont));
                    document.Add(new Paragraph($"Delivery Address: {address?.CityName ?? "N/A"}, {address?.StateName ?? "N/A"}, {address?.Pincode ?? "N/A"}")
                                         .SetFont(regularFont));

                    // --- Total Price Paragraph ---
                    Paragraph totalPriceParagraph = new Paragraph()
                        .Add(new Text("Total price of the Order "))
                        .Add(new Text($"{orderl.order.TotalPrice} Rs/- ").SetFont(boldFont));
                    document.Add(totalPriceParagraph);

                    document.Add(new Paragraph(" ")); // Add a blank line/space

                    // --- Order Items Table ---
                    Table table = new Table(new float[] { 1, 1, 2, 1, 1 }); // Relative widths: 5 columns

                    // Table headers
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Item ID").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Product ID").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Product Name").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Qty").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFont(boldFont)));

                    // Table rows
                    foreach (var item in orderl.order.OrderItems)
                    {
                        var product = products.FirstOrDefault(p => p.ProductId == item.productId);
                        string productName = product != null ? product.ProductName : "Unknown Product";

                        table.AddCell(new Cell().Add(new Paragraph(item.OrderItemId.ToString())));
                        table.AddCell(new Cell().Add(new Paragraph(item.productId.ToString())));
                        table.AddCell(new Cell().Add(new Paragraph(productName)));
                        table.AddCell(new Cell().Add(new Paragraph(item.quantity.ToString())));
                        table.AddCell(new Cell().Add(new Paragraph(item.TotalPrice.ToString())));
                    }

                    document.Add(table);

                    document.Add(new Paragraph(" ")); // Another blank line
                    document.Add(new Paragraph("Great things are not done by impulse, but by a series of small things brought together — just like your order. Thank you for being part of our journey.").SetFontSize(10));

                    document.Close(); // Close the document

                    byte[] pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Invoice_{OrderId}.pdf");
                }
            }
            else
            {
                return NotFound(); // Order not found for the given ID
            }
        }

        // --- NEW EXCEL EXPORT ACTIONS ---

        /// <summary>
        /// Exports a list of all clients along with their total order count to an Excel file.
        /// </summary>
        /// <returns>An Excel file containing client data.</returns>
        [HttpGet]
        public IActionResult ExportClientsToExcel()
        {
            List<ClientView> allClients = _clientData.GetALL(); // Call the GetClients() method from ClientData

            if (allClients == null || !allClients.Any())
            {
                return NotFound("No clients found to export.");
            }

            using (var workbook = new XLWorkbook()) // Corrected: new XLWorkbook()
            {
                var worksheet = workbook.Worksheets.Add("ClientList");

                // Headers for the Excel sheet
                worksheet.Cell("A1").Value = "Client ID";
                worksheet.Cell("B1").Value = "Client Name";
                worksheet.Cell("C1").Value = "Email";
                worksheet.Cell("D1").Value = "Role";
                worksheet.Cell("E1").Value = "Number of Orders";

                // Style Headers
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int currentRow = 2; // Start data from the second row
                foreach (var client in allClients)
                {
                    // Get the order count for each client using ProductData
                    List<Order> clientOrders = _productData.OrderForId(client.Id);
                    int orderCount = clientOrders?.Count ?? 0; // Null-coalescing to handle null if no orders

                    worksheet.Cell(currentRow, 1).Value = client.Id;
                    worksheet.Cell(currentRow, 2).Value = client.Name;
                    worksheet.Cell(currentRow, 3).Value = client.Email ?? "-"; // Use null-coalescing for potential nulls
                    worksheet.Cell(currentRow, 4).Value = client.Role ?? "-";   // Use null-coalescing for potential nulls
                    worksheet.Cell(currentRow, 5).Value = orderCount;

                    currentRow++;
                }

                worksheet.ColumnsUsed().AdjustToContents(); // Auto-adjust column widths

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream); // Save the workbook to the memory stream
                    stream.Position = 0; // Reset stream position to the beginning

                    string excelName = $"ClientList_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    // Return the file as an HTTP response
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }

        /// <summary>
        /// Exports a list of orders for a specific client to an Excel file, including order item details.
        /// </summary>
        /// <param name="clientId">The ID of the client whose orders are to be exported.</param>
        /// <returns>An Excel file containing the client's order data.</returns>
        [HttpGet]
        public IActionResult ExportClientOrdersToExcel(int clientId)
        {
            List<Order> ordersToExport = _productData.OrderForId(clientId);

            if (ordersToExport == null || !ordersToExport.Any())
            {
                return NotFound($"No orders found for client ID {clientId} to export.");
            }

            // Fetch all products once for efficient lookup of product names
            List<Product> allProducts = _productData.GetProducts();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ClientOrders");

                // Headers for the Excel sheet (including order and order item details)
                worksheet.Cell("A1").Value = "Order ID";
                worksheet.Cell("B1").Value = "Client Name";
                worksheet.Cell("C1").Value = "Delivery City";
                worksheet.Cell("D1").Value = "Delivery State";
                worksheet.Cell("E1").Value = "Delivery Pincode";
                worksheet.Cell("F1").Value = "Total Order Price (Rs)";
                worksheet.Cell("G1").Value = "Order Item ID";
                worksheet.Cell("H1").Value = "Product ID";
                worksheet.Cell("I1").Value = "Product Name";
                worksheet.Cell("J1").Value = "Quantity";
                worksheet.Cell("K1").Value = "Item Total Price";

                // Style Headers
                var headerRange = worksheet.Range("A1:K1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int currentRow = 2; // Start data from the second row
                foreach (var order in ordersToExport)
                {
                    // Retrieve client address details for the order
                    ClientDetails address = null;
                    if (order.AddressId!=0) // Check if AddressId has a value
                    {
                        address = _clientData.GetClientDetailsById(order.AddressId);
                    }

                    // Iterate through each order item for the current order
                    if (order.OrderItems != null && order.OrderItems.Any())
                    {
                        foreach (var item in order.OrderItems)
                        {
                            // Find the product name using the pre-fetched list
                            var product = allProducts.FirstOrDefault(p => p.ProductId == item.productId);
                            string productName = product != null ? product.ProductName : "N/A";

                            // Populate a row for each order item
                            worksheet.Cell(currentRow, 1).Value = order.OrderId;
                            worksheet.Cell(currentRow, 2).Value = order.ClientName;
                            worksheet.Cell(currentRow, 3).Value = address?.CityName ?? "N/A";
                            worksheet.Cell(currentRow, 4).Value = address?.StateName ?? "N/A";
                            worksheet.Cell(currentRow, 5).Value = address?.Pincode ?? "N/A";
                            worksheet.Cell(currentRow, 6).Value = order.TotalPrice;
                            worksheet.Cell(currentRow, 7).Value = item.OrderItemId;
                            worksheet.Cell(currentRow, 8).Value = item.productId;
                            worksheet.Cell(currentRow, 9).Value = productName;
                            worksheet.Cell(currentRow, 10).Value = item.quantity;
                            worksheet.Cell(currentRow, 11).Value = item.TotalPrice;

                            currentRow++; // Move to the next row for the next item
                        }
                    }
                    else
                    {
                        // If an order has no items, still list the main order info
                        worksheet.Cell(currentRow, 1).Value = order.OrderId;
                        worksheet.Cell(currentRow, 2).Value = order.ClientName;
                        worksheet.Cell(currentRow, 3).Value = address?.CityName ?? "N/A";
                        worksheet.Cell(currentRow, 4).Value = address?.StateName ?? "N/A";
                        worksheet.Cell(currentRow, 5).Value = address?.Pincode ?? "N/A";
                        worksheet.Cell(currentRow, 6).Value = order.TotalPrice;
                        worksheet.Cell(currentRow, 7).Value = "No Items"; // Indicate no items
                        worksheet.Cell(currentRow, 8).Value = "N/A";
                        worksheet.Cell(currentRow, 9).Value = "N/A";
                        worksheet.Cell(currentRow, 10).Value = "0";
                        worksheet.Cell(currentRow, 11).Value = "0";
                        currentRow++;
                    }
                }

                worksheet.ColumnsUsed().AdjustToContents(); // Auto-adjust column widths

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    // Create a dynamic filename using client name and timestamp
                    string clientNameForFileName = ordersToExport.First().ClientName.Replace(" ", "_");
                    string excelName = $"Orders_{clientNameForFileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
    }
}