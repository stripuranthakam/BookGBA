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
    public class ServicesController : Controller
    {
        private readonly GBAdbContext _context;

        public ServicesController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                string test = HttpContext.Session.GetString("employeeId");
                return View(await _context.Services.OrderBy(a => a.Name).ToListAsync());
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return View("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Services Member Index
        public async Task<IActionResult> MemberIndex()
        {
            return View(await _context.Services.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var services = await _context.Services
                    .FirstOrDefaultAsync(m => m.ServiceId == id);
                if (services == null)
                {
                    return NotFound();
                }

                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,Name,Description,TimeToComplete,Cost")] Services services)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(services);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var services = await _context.Services.FindAsync(id);
                if (services == null)
                {
                    return NotFound();
                }
                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Description,TimeToComplete,Cost")] Services services)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id != services.ServiceId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(services);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ServicesExists(services.ServiceId))
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
                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var services = await _context.Services
                    .FirstOrDefaultAsync(m => m.ServiceId == id);
                if (services == null)
                {
                    return NotFound();
                }

                return View(services);
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                var services = await _context.Services.FindAsync(id);
                _context.Services.Remove(services);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        private bool ServicesExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
