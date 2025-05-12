using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
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
            var employee = await _context.Employees
                .Include(e => e.Sales) // Include related sales data
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Prevent deletion if sales records exist
            if (employee.Sales.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete employee because they have associated sales records.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            // Or delete the sales records first
            // _context.SalesData.RemoveRange(employee.Sales);

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
