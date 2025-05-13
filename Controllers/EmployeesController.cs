using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Employees.Include(e => e.Manager);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.ManagerList = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Firstname,Lastname,DOB,DateOfHire,ManagerId")] Employee employee)
        {
            // Check for duplicate employee
            bool isDuplicate = await _context.Employees
                .AnyAsync(e => e.Firstname == employee.Firstname
                    && e.Lastname == employee.Lastname
                    && e.DOB == employee.DOB);

            if (isDuplicate)
            {
                ModelState.AddModelError("", "An employee with the same name and date of birth already exists.");
            }

            // Check if manager is same as employee
            if (employee.ManagerId == employee.EmployeeId)
            {
                ModelState.AddModelError("ManagerId", "Employee cannot be their own manager.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ManagerList = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null) return NotFound();

            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "Firstname", employee.ManagerId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Firstname,Lastname,DOB,DateOfHire,ManagerId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "Firstname", employee.ManagerId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Sales)
                    .Include(e => e.Subordinates)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found with ID: {EmployeeId}", id);
                    return NotFound();
                }

                // Log deletion attempt
                _logger.LogInformation("Attempting to delete employee {EmployeeId}: {FirstName} {LastName}",
                    employee.EmployeeId, employee.Firstname, employee.Lastname);

                // Handle subordinates (manager case)
                if (employee.Subordinates.Any())
                {
                    _logger.LogInformation("Reassigning {Count} subordinates from manager {EmployeeId}",
                        employee.Subordinates.Count, employee.EmployeeId);

                    foreach (var subordinate in employee.Subordinates)
                    {
                        subordinate.ManagerId = null;
                    }
                }

                // Delete associated sales
                if (employee.Sales.Any())
                {
                    _logger.LogInformation("Deleting {Count} sales records for employee {EmployeeId}",
                        employee.Sales.Count, employee.EmployeeId);

                    _context.SalesData.RemoveRange(employee.Sales);
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted employee {EmployeeId}", employee.EmployeeId);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error deleting employee {EmployeeId}", id);
                TempData["ErrorMessage"] = "Could not delete employee. Please try again.";
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting employee {EmployeeId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
