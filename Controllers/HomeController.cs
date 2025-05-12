using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        // AppDbContext to access the database
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? employeeId)
        {
            // Populate the employee dropdown list
            ViewBag.EmployeeList = new SelectList(await _context.Employees.ToListAsync(), "EmployeeId", "FullName");

            // Start the query to get sales data
            var salesQuery = _context.SalesData
                .Include(s => s.Employee)
                .AsQueryable();

            // Filter by employee if employeeId is provided
            if (employeeId.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.EmployeeId == employeeId.Value);
            }

            // Calculate total sales
            var totalSales = await salesQuery.SumAsync(s => s.Amount);
            ViewBag.TotalSales = totalSales;

            // Calculate quarterly sales with employee names
            var quarterlySales = await salesQuery
                .GroupBy(s => new { s.Quarter, s.Year, s.EmployeeId, s.Employee.Firstname, s.Employee.Lastname })
                .Select(g => new
                {
                    g.Key.Quarter,
                    g.Key.Year,
                    EmployeeName = $"{g.Key.Firstname} {g.Key.Lastname}",
                    TotalAmount = g.Sum(s => s.Amount)
                })
                .OrderBy(q => q.Year)
                .ThenBy(q => q.Quarter)
                .ToListAsync();

            ViewData["QuarterlySales"] = quarterlySales;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate employee data
                bool isDuplicate = await _context.Employees
                    .AnyAsync(e => e.Firstname == employee.Firstname
                        && e.Lastname == employee.Lastname
                        && e.DOB == employee.DOB);

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "An employee with the same name and date of birth already exists.");
                }

                if (employee.ManagerId == employee.EmployeeId)
                {
                    ModelState.AddModelError("ManagerId", "Employee and Manager cannot be the same person.");
                }

                // If there are any validation errors, return the view with the errors
                if (ModelState.IsValid)
                {
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employee);
        }
    }
}
