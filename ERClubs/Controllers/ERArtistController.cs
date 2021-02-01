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
    public class ERArtistController : Controller
    {
        private readonly ERClubsContext _context;

        public ERArtistController(ERClubsContext context)
        {
            _context = context;
        }

        // GET: ERArtist
        public async Task<IActionResult> Index()
        {
            //b.Sort the listing by full name
            var eRClubsContext = _context.Artist.Include(a => a.NameAddress)
                .OrderBy(s => s.NameAddress.LastName)
                .ThenBy(i => i.NameAddress.FirstName);
            return View(await eRClubsContext.ToListAsync());
        }

        // GET: ERArtist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.NameAddress)
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: ERArtist/Create
        public IActionResult Create()
        {
            ViewData["NameAddressid"] = new SelectList(_context.NameAddress, "NameAddressId", "NameAddressId");
            return View();
        }

        // POST: ERArtist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistId,MinimumHourlyRate,NameAddressid")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NameAddressid"] = new SelectList(_context.NameAddress, "NameAddressId", "NameAddressId", artist.NameAddressid);
            return View(artist);
        }

        // GET: ERArtist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            ViewData["NameAddressid"] = new SelectList(_context.NameAddress, "NameAddressId", "NameAddressId", artist.NameAddressid);
            return View(artist);
        }

        // POST: ERArtist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistId,MinimumHourlyRate,NameAddressid")] Artist artist)
        {
            if (id != artist.ArtistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ArtistId))
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
            ViewData["NameAddressid"] = new SelectList(_context.NameAddress, "NameAddressId", "NameAddressId", artist.NameAddressid);
            return View(artist);
        }

        // GET: ERArtist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .Include(a => a.NameAddress)
                .FirstOrDefaultAsync(m => m.ArtistId == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: ERArtist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artist.FindAsync(id);
            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artist.Any(e => e.ArtistId == id);
        }
    }
}
