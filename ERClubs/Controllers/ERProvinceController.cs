/* Ezatullah Rafie (8659818)
 * 
 * Created: Oct 5, 2020
 * 
 * PROG2230 - MS Web Tech
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERClubs.Models;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ERClubs.Controllers
{
    public class ERProvinceController : Controller
    {
        private readonly ERClubsContext _context;

        public ERProvinceController(ERClubsContext context)
        {
            _context = context;
        }

        // GET: ERProvince
        public async Task<IActionResult> IndexAsync(string countryCode, string countryName)
        {
            //Check if there's country code being passed
            if (!string.IsNullOrEmpty(countryCode))
            {
                //Persists the countrycode to a session
                HttpContext.Session.SetString("countryCodeSession", countryCode);

                //If the country name was passed persists it
                if (!string.IsNullOrEmpty(countryName))
                {
                    HttpContext.Session.SetString("countryNameSession", countryName);
                }
                //if country name wasnt passed then finds the country name and persists it
                else
                {
                    var country = _context.Country.FirstOrDefault(m => m.CountryCode.Equals(countryCode)).Name;
                    HttpContext.Session.SetString("countryNameSession", country);
                }
            }
            //checks if there was any country code session passed
            else if (HttpContext.Session.GetString("countryCodeSession") != null)
            {
                countryCode = HttpContext.Session.GetString("countryCodeSession");

                //Finds the country name from the found country code
                var country = _context.Country.FirstOrDefault(m => m.CountryCode.Equals(countryCode)).Name;
                HttpContext.Session.SetString("countryNameSession", country);

            }
            //if nothing was selected or passed asks the user to select a country
            else
            {
                TempData["countryNotFound"] = "Please select a country";
                Response.Redirect("ERCountry");
            }

            //Displays the provinces/states of a selected country
            var provinceCountry = await _context.Province.Where(c => c.CountryCode == countryCode).ToListAsync();
            return View(provinceCountry.OrderBy(s => s.Name));
        }

        // GET: ERProvince/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        // GET: ERProvince/Create
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode");
            return View();
        }

        // POST: ERProvince/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            //passes country country code to create new record
            province.CountryCode = HttpContext.Session.GetString("countryCodeSession");

            if (ModelState.IsValid)
            {
                //Checking for duplicate province records
                if (ProvinceExists(province.ProvinceCode))
                {
                    TempData["duplicateProvince"] = "The entered province code already exists";
                    return View();
                }
                else if (_context.Province.Any(m => m.Name == province.Name))
                {
                    TempData["duplicateProvince"] = "The entered province name already exists";
                    return View();
                }
                //If there was no duplicate
                else
                {
                    _context.Add(province);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexAsync));
                }
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        // GET: ERProvince/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.FindAsync(id);
            if (province == null)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        // POST: ERProvince/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Name,CountryCode,SalesTaxCode,SalesTax,IncludesFederalTax,FirstPostalLetter")] Province province)
        {
            //Passes the country code that was saved in session to edit a record without allowing the user to edit
            province.CountryCode = HttpContext.Session.GetString("countryCodeSession");

            if (id != province.ProvinceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexAsync));
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        // GET: ERProvince/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .FirstOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        // POST: ERProvince/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var province = await _context.Province.FindAsync(id);
            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAsync));
        }

        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
