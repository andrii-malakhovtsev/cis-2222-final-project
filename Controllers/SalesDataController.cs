using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class SalesDataController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public SalesDataController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: SalesData
        public async Task<IActionResult> Index(int? employeeId)
        {
            ViewBag.EmployeeList = new SelectList(_context.Employees, "EmployeeId", "FullName");

            var salesQuery = _context.SalesData.Include(s => s.Employee).AsQueryable();

            if (employeeId.HasValue)
                salesQuery = salesQuery.Where(s => s.EmployeeId == employeeId.Value);

            return View(await salesQuery.ToListAsync());
        }

        // GET: SalesData/Create
        public IActionResult Create()
        {
            ViewBag.ManagerList = new SelectList(_context.Employees, "EmployeeId", "FullName");
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // POST: SalesData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesDataId,Quarter,Year,Amount,EmployeeId")] SalesData salesData)
        {
            await ValidateSalesData(salesData);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(salesData);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error creating sales record");
                    ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                }
            }

            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "FullName", salesData.EmployeeId);
            return View(salesData);
        }

        // GET: SalesData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesData = await _context.SalesData.FindAsync(id);
            if (salesData == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Firstname", salesData.EmployeeId);
            return View(salesData);
        }

        // POST: SalesData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalesDataId,Quarter,Year,Amount,EmployeeId")] SalesData salesData)
        {
            if (id != salesData.SalesDataId)
            {
                return NotFound();
            }

            await ValidateSalesData(salesData, isEdit: true);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesData);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!SalesDataExists(salesData.SalesDataId))
                    {
                        return NotFound();
                    }
                    _logger.LogError(ex, "Concurrency error editing sales record");
                    ModelState.AddModelError("", "The record was modified by another user. Please refresh and try again.");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error editing sales record");
                    ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                }
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", salesData.EmployeeId);
            return View(salesData);
        }

        // GET: SalesData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesData = await _context.SalesData
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalesDataId == id);

            if (salesData == null)
            {
                return NotFound();
            }

            return View(salesData);
        }

        // POST: SalesData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesData = await _context.SalesData.FindAsync(id);
            if (salesData != null)
            {
                _context.SalesData.Remove(salesData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesDataExists(int id)
        {
            return _context.SalesData.Any(e => e.SalesDataId == id);
        }

        private async Task ValidateSalesData(SalesData salesData, bool isEdit = false)
        {
            // Load employee if not already loaded
            var employee = await _context.Employees.FindAsync(salesData.EmployeeId);

            // Check for duplicates (excluding current record if editing)
            var duplicateQuery = _context.SalesData
                .Where(s => s.Quarter == salesData.Quarter
                         && s.Year == salesData.Year
                         && s.EmployeeId == salesData.EmployeeId);

            if (isEdit)
            {
                duplicateQuery = duplicateQuery.Where(s => s.SalesDataId != salesData.SalesDataId);
            }

            if (await duplicateQuery.AnyAsync())
            {
                ModelState.AddModelError("", "Sales data for this employee, quarter, and year already exists.");
            }

            // Validate sale date vs hire date
            if (employee != null)
            {
                var saleDate = new DateTime(salesData.Year, (salesData.Quarter - 1) * 3 + 1, 1);
                if (saleDate < employee.DateOfHire)
                {
                    ModelState.AddModelError("", $"Cannot create sales before employee's hire date ({employee.DateOfHire:yyyy-MM-dd})");
                }
            }

            if (salesData.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero.");
            }

            if (salesData.Year > DateTime.Today.Year)
            {
                ModelState.AddModelError("Year", "Year cannot be in the future.");
            }
        }
    }
}
