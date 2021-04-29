using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBA_Application.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace GBA_Application.Controllers
{
    public class OrdersController : Controller
    {
        private readonly GBAdbContext _context;

        public OrdersController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                var gBAdbContext = _context.Orders.Include(o => o.Appointment).Include(a => a.OrderStatuses);
                return View(await gBAdbContext.ToListAsync());
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        //Get ORders for member view
        public async Task<IActionResult> MemberIndex()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var memberId = HttpContext.Session.GetString("memberId");

                var gBAdbContext = _context.Orders.Include(o => o.Appointment).ThenInclude(a => a.Service).Include(a => a.OrderStatuses).ThenInclude(a => a.Employee).Where(a => a.Appointment.MemberId == int.Parse(memberId));
                return View(await gBAdbContext.ToListAsync());
            }
            else if (HttpContext.Session.GetString("employeeId") != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var orders = await _context.Orders
                    .Include(o => o.Appointment)
                    .FirstOrDefaultAsync(m => m.OrderId == id);
                if (orders == null)
                {
                    return NotFound();
                }

                return View(orders);
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

        // GET: Orders/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "ApptInfo");
                return View();
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

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,AppointmentId,OrderDate,OrderTotal,OrderDescription,Completed")] Orders orders)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(orders);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", orders.AppointmentId);
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "ApptInfo", orders.AppointmentId);
                return View(orders);
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

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var orders = await _context.Orders.FindAsync(id);
                if (orders == null)
                {
                    return NotFound();
                }
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "ApptInfo", orders.AppointmentId);
                return View(orders);
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

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,AppointmentId,OrderDate,OrderTotal,OrderDescription,Completed")] Orders orders)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id != orders.OrderId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(orders);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrdersExists(orders.OrderId))
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
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "ApptInfo", orders.AppointmentId);
                return View(orders);
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

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var orders = await _context.Orders
                    .Include(o => o.Appointment)
                    .FirstOrDefaultAsync(m => m.OrderId == id);
                if (orders == null)
                {
                    return NotFound();
                }

                return View(orders);
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

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //delete order status first
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).AsNoTracking().FirstOrDefault();
                _context.Remove(orderStatus);

                var orders = await _context.Orders.FindAsync(id);
                _context.Orders.Remove(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        //// GET: Status
        //public IActionResult ChooseStatus()
        //{
        //    if (HttpContext.Session.GetString("employeeId") != null)
        //    {
        //        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "ApptInfo");
        //        return View();
        //    }
        //    else if (HttpContext.Session.GetString("memberId") != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Members");
        //    }
        //}

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChooseStatus(string status)
        {
            //check which status is picekd then send to that method
            //texting thing
            
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //split on dash
                string[] arr = status.Split('-');
                
                switch (arr[0])
                {
                    case "started":
                        await OrderStarted(int.Parse(arr[1]));
                        break;
                    case "delayed":
                        await Delayed(int.Parse(arr[1]));
                        break;
                    case "progress":
                        await InProgress(int.Parse(arr[1]));
                        break;
                    case "contact":
                        await ContactCustomer(int.Parse(arr[1]));
                        break;
                    case "cancelled":
                        await Cancelled(int.Parse(arr[1]));
                        break;
                    case "completed":
                        await Completed(int.Parse(arr[1]));
                        break;
                    default:
                        break;
                }

                return RedirectToAction("Index");
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

        // GET: Mark Order as Started
        public async Task<IActionResult> OrderStarted(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //orderId, StatusId, EmployeeId, EstimatedFinishDate, StartDate
                var orderId = id;
                var statusId = 1;
                var employeeId = _context.Employees.FirstOrDefault(a => a.EmployeeId == int.Parse(HttpContext.Session.GetString("employeeId")));
                var estFinishDate = DateTime.Today.AddDays(14);
                var startDate = DateTime.Today;

                _context.OrderStatuses.Add(new OrderStatuses { OrderId = orderId, StatusId = statusId, EmployeeId = employeeId.EmployeeId, EstimatedFinishDate = estFinishDate, StartDate = startDate });
                await _context.SaveChangesAsync();

                //send email, name, email, status
                var order = _context.Orders.Where(a => a.OrderId == id).AsNoTracking().FirstOrDefault();
                var aptId = order.AppointmentId;
                var appt = _context.Appointments.Where(a => a.AppointmentId == aptId).AsNoTracking().Include(a => a.Member).FirstOrDefault();
                var member = appt.Member;

                SendEmail(1, member.Email, member.fullName);

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Mark Order as Delayed
        public async Task<IActionResult> Delayed(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 2, and add 7 days to est finish date
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).FirstOrDefault();
                
                var statusId = 2;
                var estFinishDate = orderStatus.EstimatedFinishDate.AddDays(7);

                //update
                orderStatus.StatusId = statusId;
                orderStatus.EstimatedFinishDate = estFinishDate;

                _context.Update(orderStatus);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Mark Order as In Progress
        public async Task<IActionResult> InProgress(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 6
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).FirstOrDefault();

                var statusId = 6;

                //update
                orderStatus.StatusId = statusId;

                _context.Update(orderStatus);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Mark Order as Contact Customer
        public async Task<IActionResult> ContactCustomer(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 3
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).FirstOrDefault();

                var statusId = 3;

                //update
                orderStatus.StatusId = statusId;

                _context.Update(orderStatus);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Mark Order as Cancelled
        public async Task<IActionResult> Cancelled(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 5
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).FirstOrDefault();

                var statusId = 5;

                //update
                orderStatus.StatusId = statusId;

                _context.Update(orderStatus);
                await _context.SaveChangesAsync();

                //send email, name, email, status
                var order = _context.Orders.Where(a => a.OrderId == id).AsNoTracking().FirstOrDefault();
                var aptId = order.AppointmentId;
                var appt = _context.Appointments.Where(a => a.AppointmentId == aptId).AsNoTracking().Include(a => a.Member).FirstOrDefault();
                var member = appt.Member;

                SendEmail(5, member.Email, member.fullName);

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Mark Order as Completed
        public async Task<IActionResult> Completed(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 4, end date
                var orderStatus = _context.OrderStatuses.Where(a => a.OrderId == id).FirstOrDefault();

                var statusId = 4;
                var finishDate = DateTime.Today;

                //update
                orderStatus.StatusId = statusId;
                orderStatus.FinishDate = finishDate;

                _context.Update(orderStatus);

                //mark order as completed
                var order = _context.Orders.Where(a => a.OrderId == id).AsNoTracking().FirstOrDefault();
                order.Completed = true;
                _context.Update(order);

                await _context.SaveChangesAsync();

                //send email, name, email, status
                var aptId = order.AppointmentId;
                var appt = _context.Appointments.Where(a => a.AppointmentId == aptId).AsNoTracking().Include(a => a.Member).FirstOrDefault();
                var member = appt.Member;

                SendEmail(4, member.Email, member.fullName);

                return RedirectToAction(nameof(Index));
            }
            else if (HttpContext.Session.GetString("memberId") != null)
            {
                return RedirectToAction("MemberIndex");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        public void SendEmail(int statusId, string userEmail, string name)
        {
            //edit//cancel//create
            string message = "";
            switch (statusId)
            {
                case 1:
                    message = "Hi " + name + "! We have started working on your vehicle, and it should be ready soon!";
                    break;
                case 4:
                    message = "Hi " + name + "! We have completed your order, and your vehicle is ready for pickup.";
                    break;
                case 5:
                    message = "Hi " + name + "! Your order has been cancelled.";
                    break;
                default:
                    break;
            }

            MailMessage mail = new MailMessage();

            mail.To.Add(userEmail);
            mail.From = new MailAddress("shfsgames@gmail.com", "SHFS Admin", System.Text.Encoding.UTF8);
            mail.Subject = "GBA Motors - Vehicle Status";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.Body = message;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("shfsgames@gmail.com", "ShfsGames687.");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message;
            }

            //Send text message using Twilio
            var member = _context.Members.Where(a => a.Email == userEmail).FirstOrDefault();

            if (member != null)
            {
                var phoneNumber = member.Phone;
                if (phoneNumber != null)
                {
                    string sid = "AC4a94e75fc10600e785665fe822452de8";
                    string authToken = "d158c1b9e6e87bdc7fcacb3eabe20325";

                    TwilioClient.Init(sid, authToken);

                    var messageText = MessageResource.Create(
                        body: message + " -GBA Motors",
                        from: new Twilio.Types.PhoneNumber("2898061596"),
                        to: new Twilio.Types.PhoneNumber(phoneNumber)
                        );
                }
            }
        }
    }
}
