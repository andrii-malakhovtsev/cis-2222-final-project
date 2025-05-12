using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Controllers
{
    public class SalesDataController : Controller
    {
        private readonly AppDbContext _context;

        public SalesDataController(AppDbContext context)
        {
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
            // Check for duplicate sales data
            bool isDuplicate = await _context.SalesData
                .AnyAsync(s => s.Quarter == salesData.Quarter
                    && s.Year == salesData.Year
                    && s.EmployeeId == salesData.EmployeeId);

            if (isDuplicate)
            {
                ModelState.AddModelError("", "Sales data for this employee, quarter, and year already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(salesData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesDataExists(salesData.SalesDataId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Firstname", salesData.EmployeeId);
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
    }
}
