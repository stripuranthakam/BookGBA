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
    public class OrderStatusesController : Controller
    {
        private readonly GBAdbContext _context;

        public OrderStatusesController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: OrderStatuses
        public async Task<IActionResult> Index()
        {
            var gBAdbContext = _context.OrderStatuses.Include(o => o.Employee).Include(o => o.Order).Include(o => o.Status);
            return View(await gBAdbContext.ToListAsync());
        }

        // GET: OrderStatuses
        public async Task<IActionResult> Status(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var gBAdbContext = await _context.OrderStatuses
                    .Include(o => o.Employee)
                    .Include(o => o.Order)
                    .Include(o => o.Status)
                    .FirstOrDefaultAsync(m => m.OrderStatusesId == id);
                if (gBAdbContext == null)
                {
                    return NotFound();
                }

                return View(gBAdbContext);
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: OrderStatuses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderStatuses = await _context.OrderStatuses
                .Include(o => o.Employee)
                .Include(o => o.Order)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.OrderStatusesId == id);
            if (orderStatuses == null)
            {
                return NotFound();
            }

            return View(orderStatuses);
        }

        // GET: OrderStatuses/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId");
            return View();
        }

        // POST: OrderStatuses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderStatusesId,OrderId,StatusId,EmployeeId,EstimatedFinishDate,StartDate,FinishDate")] OrderStatuses orderStatuses)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderStatuses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", orderStatuses.EmployeeId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderStatuses.OrderId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", orderStatuses.StatusId);
            return View(orderStatuses);
        }

        // GET: OrderStatuses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderStatuses = await _context.OrderStatuses.FindAsync(id);
            if (orderStatuses == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", orderStatuses.EmployeeId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderStatuses.OrderId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", orderStatuses.StatusId);
            return View(orderStatuses);
        }

        // POST: OrderStatuses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderStatusesId,OrderId,StatusId,EmployeeId,EstimatedFinishDate,StartDate,FinishDate")] OrderStatuses orderStatuses)
        {
            if (id != orderStatuses.OrderStatusesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderStatuses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderStatusesExists(orderStatuses.OrderStatusesId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", orderStatuses.EmployeeId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderStatuses.OrderId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", orderStatuses.StatusId);
            return View(orderStatuses);
        }

        // GET: OrderStatuses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderStatuses = await _context.OrderStatuses
                .Include(o => o.Employee)
                .Include(o => o.Order)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.OrderStatusesId == id);
            if (orderStatuses == null)
            {
                return NotFound();
            }

            return View(orderStatuses);
        }

        // POST: OrderStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderStatuses = await _context.OrderStatuses.FindAsync(id);
            _context.OrderStatuses.Remove(orderStatuses);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderStatusesExists(int id)
        {
            return _context.OrderStatuses.Any(e => e.OrderStatusesId == id);
        }
    }
}
