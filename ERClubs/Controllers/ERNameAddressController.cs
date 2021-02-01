using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERClubs.Models;

namespace ERClubs.Controllers
{
    public class ERNameAddressController : Controller
    {
        private readonly ERClubsContext _context;

        public ERNameAddressController(ERClubsContext context)
        {
            _context = context;
        }

        // GET: ERNameAddress
        public async Task<IActionResult> Index()
        {
            var eRClubsContext = _context.NameAddress.Include(n => n.ProvinceCodeNavigation);
            return View(await eRClubsContext.ToListAsync());
        }

        // GET: ERNameAddress/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress
                .Include(n => n.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.NameAddressId == id);
            if (nameAddress == null)
            {
                return NotFound();
            }

            return View(nameAddress);
        }

        // GET: ERNameAddress/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: ERNameAddress/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameAddressId,FirstName,LastName,CompanyName,StreetAddress,City,PostalCode,ProvinceCode,Email,Phone")] NameAddress nameAddress)
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", nameAddress.ProvinceCode);
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(nameAddress);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Successfully Saved";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error adding new record: {ex.GetBaseException().Message}");
            }
            return View(nameAddress);
        }

        // GET: ERNameAddress/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress.FindAsync(id);
            if (nameAddress == null)
            {
                return NotFound();
            }
            ViewData["ProvinceName"] = new SelectList(_context.Province, "ProvinceCode", "Name", nameAddress.ProvinceCodeNavigation);
            return View(nameAddress);
        }

        // POST: ERNameAddress/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NameAddressId,FirstName,LastName,CompanyName,StreetAddress,City,PostalCode,ProvinceCode,Email,Phone")] NameAddress nameAddress)
        {
            if (id != nameAddress.NameAddressId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(nameAddress);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!NameAddressExists(nameAddress.NameAddressId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["SuccessMessage"] = "Successfully Saved";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"error saving the edits: {ex.GetBaseException().Message}");
            }
            ViewData["ProvinceName"] = new SelectList(_context.Province, "ProvinceCode", "Name", nameAddress.ProvinceCodeNavigation);
            return View(nameAddress);
        }

        // GET: ERNameAddress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameAddress = await _context.NameAddress
                .Include(n => n.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.NameAddressId == id);
            if (nameAddress == null)
            {
                return NotFound();
            }

            return View(nameAddress);
        }

        // POST: ERNameAddress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var nameAddress = await _context.NameAddress.FindAsync(id);
                _context.NameAddress.Remove(nameAddress);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully Deleted";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["DeleteMessage"] = $"error deleting the record: {ex.GetBaseException().Message}";
                return RedirectToAction(nameof(Delete));
            }
        }

        private bool NameAddressExists(int id)
        {
            return _context.NameAddress.Any(e => e.NameAddressId == id);
        }
    }
}
