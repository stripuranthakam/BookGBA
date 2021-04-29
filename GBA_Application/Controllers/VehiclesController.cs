using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBA_Application.Models;
using Microsoft.AspNetCore.Http;

namespace GBA_Application.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly GBAdbContext _context;

        public VehiclesController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var memberId = HttpContext.Session.GetString("memberId");

                var gBAdbContext = _context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)).OrderBy(a => a.Make).ThenBy(a => a.Model).ThenBy(a => a.Year);
                return View(await gBAdbContext.ToListAsync());
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Home", "Index");
            }
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var vehicles = await _context.Vehicles
                    .FirstOrDefaultAsync(m => m.VehicleId == id);
                if (vehicles == null)
                {
                    return NotFound();
                }

                return View(vehicles);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                return View();
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Vehicles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,Year,Make,Model,LicensePlate,MemberId")] Vehicles vehicles)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (ModelState.IsValid)
                {
                    var memberId = HttpContext.Session.GetString("memberId");
                    vehicles.MemberId = int.Parse(memberId);

                    _context.Add(vehicles);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(vehicles);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var vehicles = await _context.Vehicles.FindAsync(id);
                if (vehicles == null)
                {
                    return NotFound();
                }
                return View(vehicles);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Vehicles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,Year,Make,Model,LicensePlate,MemberId")] Vehicles vehicles)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id != vehicles.VehicleId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var memberId = HttpContext.Session.GetString("memberId");
                        vehicles.MemberId = int.Parse(memberId);

                        _context.Update(vehicles);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!VehiclesExists(vehicles.VehicleId))
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
                return View(vehicles);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var vehicles = await _context.Vehicles
                    .FirstOrDefaultAsync(m => m.VehicleId == id);
                if (vehicles == null)
                {
                    return NotFound();
                }

                return View(vehicles);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var vehicles = await _context.Vehicles.FindAsync(id);
                _context.Vehicles.Remove(vehicles);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return RedirectToAction("Login", "Members");
            }
        }

        private bool VehiclesExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
