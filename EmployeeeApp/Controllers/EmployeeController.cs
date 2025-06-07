using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeeApp.Models;
using EmployeeeApp.Data;

namespace EmployeeeApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeData _employeeData;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(IWebHostEnvironment webHostEnvironment)
        {
            _employeeData = new EmployeeData();
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            List<Employee> employees = _employeeData.GetAll(page, pageSize);
            int totalEmployees = _employeeData.GetTotalCount();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);
            ViewBag.CurrentPage = page;

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee, IFormFile? profileImage)
        {
            if (ModelState.IsValid)
            {
                if (profileImage != null && profileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    profileImage.CopyTo(fileStream);

                    employee.Profilepic = "/images/" + fileName;
                }
                else
                {
                    employee.Profilepic = "/images/default_image.jpg";
                }

                if (_employeeData.Insert(employee))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(employee);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee = _employeeData.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(Employee employee, IFormFile? profileImage)
        {
            if (ModelState.IsValid)
            {
                if (profileImage != null && profileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    profileImage.CopyTo(fileStream);

                    employee.Profilepic = "/images/" + fileName;
                }

                if (_employeeData.Update(employee))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(employee);
        }

        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Employee employee = _employeeData.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_employeeData.Delete(id))
            {
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}