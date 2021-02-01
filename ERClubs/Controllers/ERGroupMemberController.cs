using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERClubs.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;

namespace ERClubs.Controllers
{
    public class ERGroupMemberController : Controller
    {
        private readonly ERClubsContext _context;

        public ERGroupMemberController(ERClubsContext context)
        {
            _context = context;
        }

        // GET: ERGroupMember
        public async Task<IActionResult> Index(string artistId, string firstName, string lastName)
        {
            //a.If the artistId is in the URL or a QueryString variable, save it to a cookie or session variabl
            if (!string.IsNullOrEmpty(artistId))
            {
                HttpContext.Session.SetString("artistIdSession", artistId);
            }
            else
            {
                //If no artistId was passed in the URL or QueryString, look for a cookie or session variable with it, and use that
                if (HttpContext.Session.GetString("artistIdSession") != null)
                {
                    artistId = HttpContext.Session.GetString("artistIdSession");
                }
                else
                {
                    //If there’s no artistIdin either a cookie or session variable, return to the XXArtistController witha message asking them to select an artis
                    TempData["Temp_Data"] = "Please select an artist";
                    return RedirectToAction("Index", "ERArtist");
                }
            }
            
            var artistGroup = _context.GroupMember.Where(k => k.ArtistIdGroup == Convert.ToInt32(artistId));
            var individual = _context.GroupMember.Where(k => k.ArtistIdMember == Convert.ToInt32(artistId));

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            //If the artistId is in the ArtistIdGroup field of any groupMember record:
            if (artistGroup.Count() != 0)
            {
                //i.Filter the groupMember records to ones that have the given artistId in the field artistIdGroup.
                var eRClubsContext = _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g => g.ArtistIdMemberNavigation)
                .Include(h=>h.ArtistIdMemberNavigation.NameAddress)
                .Where(h => h.ArtistIdGroup == Convert.ToInt32(artistId))
                .OrderBy(h=>h.DateLeft).ThenBy(h=>h.DateJoined);

                return View(await eRClubsContext.ToListAsync());

            }
            else if(individual.Count() != 0)
            {

                TempData["Temp_Data"] = "Artist is an individual, not a group, here's their historic group memberships";
                return RedirectToAction("GroupsForArtist", "ERGroupMember");
            }
            else
            {
                TempData["Temp_Data"] = "the artist is neither a group nor a group member, but they can become a group";
                return RedirectToAction("Create", "ERGroupMember");
            }
            
        }
        public async Task<IActionResult> GroupsForArtist(string artistId,  string firstName, string lastName)
        {
            //a.If the artistId is in the URL or a QueryString variable, save it to a cookie or session variabl
            if (!string.IsNullOrEmpty(artistId))
            {
                HttpContext.Session.SetString("artistIdSession", artistId);
            }
            else
            {
                //If no artistId was passed in the URL or QueryString, look for a cookie or session variable with it, and use that
                if (HttpContext.Session.GetString("artistIdSession") != null)
                {
                    artistId = HttpContext.Session.GetString("artistIdSession");
                }
                else
                {
                    //If there’s no artistIdin either a cookie or session variable, return to the XXArtistController witha message asking them to select an artis
                    TempData["Temp_Data"] = "Please select an artist";
                    return RedirectToAction("Index", "ERArtist");
                }
            }

            var eRClubsContext = _context.GroupMember
            .Include(g => g.ArtistIdGroupNavigation)
            .Include(g => g.ArtistIdMemberNavigation)
            .Include(h => h.ArtistIdGroupNavigation.NameAddress)
            .Where(h => h.ArtistIdMember == Convert.ToInt32(artistId))
            .OrderBy(h => h.DateLeft).ThenBy(h => h.DateJoined);

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            //ViewData["Names"] = eRClubsContext.FirstOrDefault(a => a.ArtistIdMember == Convert.ToInt32(artistId)).ArtistIdMemberNavigation.NameAddress.FirstName
            //    + " " + eRClubsContext.FirstOrDefault(a => a.ArtistIdMember == Convert.ToInt32(artistId)).ArtistIdMemberNavigation.NameAddress.LastName;
            return View(await eRClubsContext.ToListAsync());
        }


        // GET: ERGroupMember/Details/5
        public async Task<IActionResult> Details(int? memberID, int? groupId, string firstName, string lastName)
        {

            var groupMember = await _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g => g.ArtistIdMemberNavigation)
                .Where(g=>g.ArtistIdMember == memberID)
                .FirstOrDefaultAsync(m => m.ArtistIdGroup == groupId);
            if (groupMember == null)
            {
                return NotFound();
            }
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            return View(groupMember);
        }

        // GET: ERGroupMember/Create
        public IActionResult Create(string firstName, string lastName)
        {
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            int artistId = Convert.ToInt32(HttpContext.Session.GetString("artistIdSession"));
            var artist = _context.Artist.Include(g => g.NameAddress)
                .Where(g => g.GroupMemberArtistIdGroupNavigation.Count() == 0)
                .Where(g => g.GroupMemberArtistIdMemberNavigation.Count() == 0)
                .Where(g => g.ArtistId != artistId)
                .Select(g=> new
                {
                    Key = g.ArtistId,
                    Name = String.Concat(g.NameAddress.FirstName, " ", g.NameAddress.LastName)
                })
                .ToList();
            var groupId = _context.Artist.Include(g => g.NameAddress)
                .Where(g => g.ArtistId == artistId);
            ViewData["ArtistIdGroup"] = new SelectList(groupId, "ArtistId", "ArtistId");
            ViewData["ArtistIdMember"] = new SelectList(artist, "Key", "Name");
            return View();
        }

        // POST: ERGroupMember/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistIdGroup,ArtistIdMember,DateJoined,DateLeft")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdGroup);
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdMember);
            return View(groupMember);
        }

        // GET: ERGroupMember/Edit/5
        public async Task<IActionResult> Edit(int? groupId, int? memberId, string firstName, string lastName)
        {
            if (groupId == null)
            {
                return NotFound();
            }
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            var groupMember = await _context.GroupMember.Where(a=>a.ArtistIdGroup == groupId).FirstOrDefaultAsync(a=>a.ArtistIdMember == memberId);
            if (groupMember == null)
            {
                return NotFound();
            }
            ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdGroup);
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdMember);
            return View(groupMember);
        }

        // POST: ERGroupMember/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int groupId, int memberId, [Bind("ArtistIdGroup,ArtistIdMember,DateJoined,DateLeft")] GroupMember groupMember)
        {
            groupMember.ArtistIdGroup = groupId;
            groupMember.ArtistIdMember = memberId;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupMemberExists(groupMember.ArtistIdGroup))
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
            ViewData["ArtistIdGroup"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdGroup);
            ViewData["ArtistIdMember"] = new SelectList(_context.Artist, "ArtistId", "ArtistId", groupMember.ArtistIdMember);
            return View(groupMember);
        }

        // GET: ERGroupMember/Delete/5
        public async Task<IActionResult> Delete(int? groupId, int? memberId, string firstName, string lastName)
        {
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            var groupMember = await _context.GroupMember
                .Include(g => g.ArtistIdGroupNavigation)
                .Include(g => g.ArtistIdMemberNavigation)
                .Where(g=>g.ArtistIdMember == memberId)
                .FirstOrDefaultAsync(m => m.ArtistIdGroup == groupId);
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember);
        }

        // POST: ERGroupMember/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupMember = await _context.GroupMember.FindAsync(id);
            _context.GroupMember.Remove(groupMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupMemberExists(int id)
        {
            return _context.GroupMember.Any(e => e.ArtistIdGroup == id);
        }
    }
}
