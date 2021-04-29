using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBA_Application.Models;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace GBA_Application.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly GBAdbContext _context;

        public AppointmentsController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Appointments - Employee View all upcoming appointments 
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                var gBAdbContext = _context.Appointments.Include(a => a.Member).Include(a => a.Service).Include(a => a.Vehicle).Where(a => a.AppointmentDate > DateTime.Now).OrderByDescending(a => a.AppointmentDate);
                return View(await gBAdbContext.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Appointments - Employee View old upcoming appointments 
        public async Task<IActionResult> OldAppointments()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                var gBAdbContext = _context.Appointments.Include(a => a.Member).Include(a => a.Service).Include(a => a.Vehicle).Where(a => a.AppointmentDate < DateTime.Now).OrderByDescending(a => a.AppointmentDate);
                return View(await gBAdbContext.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Appointments - Member view all appointments
        public async Task<IActionResult> MemberIndex()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var memberId = HttpContext.Session.GetString("memberId");

                var gBAdbContext = _context.Appointments.Include(a => a.Member).Include(a => a.Service).Include(a => a.Vehicle).Where(a => a.MemberId == int.Parse(memberId)).OrderByDescending(a => a.AppointmentDate);

                return View(await gBAdbContext.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("memberId") != null || HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appointments = await _context.Appointments
                    .Include(a => a.Member)
                    .Include(a => a.Service)
                    .Include(a => a.Vehicle)
                    .FirstOrDefaultAsync(m => m.AppointmentId == id);
                if (appointments == null)
                {
                    return NotFound();
                }

                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        // GET: Appointments/Create for employee
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {

                ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "fullName");
                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,MemberId,ServiceId,AppointmentDate,AppointmentTime,Description")] Appointments appointments)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {

                if (ModelState.IsValid)
                {
                    if (DateTime.Today < appointments.AppointmentDate)
                    {
                        appointments.Approved = true;
                        _context.Add(appointments);
                        await _context.SaveChangesAsync();

                        //Email customer
                        var member = _context.Members.Where(a => a.MemberId == appointments.MemberId).FirstOrDefault();
                        var email = member.Email;
                        var fname = member.FirstName;
                        var lname = member.LastName;

                        SendEmail("Created", email, fname + " " + lname, appointments.AppointmentDate, appointments.AppointmentTime);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.dateError = "Choose any date after today";
                    }
                }
                ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "fullName", appointments.MemberId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointments.ServiceId);
                //ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)), "VehicleId", "fullVehicleName", appointments.VehicleId);
                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Appointments/Create for member
        public IActionResult CreateMember()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var memberId = HttpContext.Session.GetString("memberId");

                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name");
                ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)), "VehicleId", "fullVehicleName");
                return View();
            }
            else if (HttpContext.Session.GetString("employeeId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Appointments/Create Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember([Bind("AppointmentId,ServiceId,VehicleId,AppointmentDate,AppointmentTime,Description")] Appointments appointments)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                var memberId = HttpContext.Session.GetString("memberId");
                
                if (ModelState.IsValid)
                {

                    //validate date and time
                    if (DateTime.Today < appointments.AppointmentDate)
                    {
                        appointments.MemberId = int.Parse(memberId);
                        appointments.Approved = false;
                        _context.Add(appointments);
                        await _context.SaveChangesAsync();

                        //Email customer
                        var member = _context.Members.Where(a => a.MemberId == int.Parse(memberId)).FirstOrDefault();
                        var email = member.Email;
                        var fname = member.FirstName;
                        var lname = member.LastName;

                        //SendEmail("Created", email, fname + " " + lname, appointments.AppointmentDate, appointments.AppointmentTime);

                        return RedirectToAction(nameof(MemberIndex));
                    }
                    else
                    {
                        ViewBag.dateError = "Choose any date after today";
                    }
                }
                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointments.ServiceId);
                ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)), "VehicleId", "fullVehicleName", appointments.VehicleId);
                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Appointments/Edit/5 - only employees can edit appointments, sends member an email !
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appointments = await _context.Appointments.FindAsync(id);
                if (appointments == null)
                {
                    return NotFound();
                }
                ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "fullName", appointments.MemberId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointments.ServiceId);
                //ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)), "VehicleId", "fullVehicleName", appointments.VehicleId);
                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,MemberId,ServiceId,AppointmentDate,AppointmentTime,Description")] Appointments appointments)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                if (id != appointments.AppointmentId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    if (DateTime.Today < appointments.AppointmentDate)
                    {
                        try
                        {
                            var apt = _context.Appointments.Where(a => a.AppointmentId == id).AsNoTracking().FirstOrDefault();
                            var memberId = apt.MemberId;
                            appointments.MemberId = memberId;
                            appointments.Approved = true;

                            _context.Update(appointments);
                            await _context.SaveChangesAsync();

                            //EMAIL CUSTOMER OF UPDATED APPOINTMENT!
                            var member = _context.Members.Where(a => a.MemberId == memberId).FirstOrDefault();
                            var email = member.Email;
                            var fname = member.FirstName;
                            var lname = member.LastName;

                            SendEmail("Changed", email, fname + " " + lname, appointments.AppointmentDate, appointments.AppointmentTime);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!AppointmentsExists(appointments.AppointmentId))
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
                    else
                    {
                        ViewBag.dateError = "Choose any date after today";
                    }
                }
                ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "fullName", appointments.MemberId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointments.ServiceId);
                //ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(a => a.MemberId == int.Parse(memberId)), "VehicleId", "fullVehicleName", appointments.VehicleId);
                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //Approve appointment
        public async Task<IActionResult> Approve(int id)
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                //find orderstatus Id and change status id to 3
                var appointment = _context.Appointments.Where(a => a.AppointmentId == id).AsNoTracking().FirstOrDefault();

                //update
                appointment.Approved = true;

                _context.Update(appointment);
                await _context.SaveChangesAsync();

                //Send Email
                //EMAIL CUSTOMER OF UPDATED APPOINTMENT!
                var member = _context.Members.Where(a => a.MemberId == appointment.MemberId).FirstOrDefault();
                var email = member.Email;
                var fname = member.FirstName;
                var lname = member.LastName;

                SendEmail("Approved", email, fname + " " + lname, appointment.AppointmentDate, appointment.AppointmentTime);

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

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("employeeId") != null || HttpContext.Session.GetString("memberId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appointments = await _context.Appointments
                    .Include(a => a.Member)
                    .Include(a => a.Service)
                    .Include(a => a.Vehicle)
                    .FirstOrDefaultAsync(m => m.AppointmentId == id);
                if (appointments == null)
                {
                    return NotFound();
                }

                return View(appointments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("employeeId") != null || HttpContext.Session.GetString("memberId") != null)
            {
                //EMAIL CUSTOMER OF UPDATED APPOINTMENT!
                var apt = _context.Appointments.Where(a => a.AppointmentId == id).FirstOrDefault();
                var memberId = apt.MemberId;
                var member = _context.Members.Where(a => a.MemberId == memberId).FirstOrDefault();
                var email = member.Email;
                var fname = member.FirstName;
                var lname = member.LastName;

                SendEmail("Cancelled", email, fname + " " + lname);

                var appointments = await _context.Appointments.FindAsync(id);
                _context.Appointments.Remove(appointments);
                await _context.SaveChangesAsync();
                if (HttpContext.Session.GetString("employeeId") != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(MemberIndex));
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private bool AppointmentsExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        
        public void SendEmail(string type, string userEmail, string name, DateTime aptDate = default, TimeSpan aptTime = default)
        {
            //edit//cancel//create
            string message = "";
            switch (type)
            {
                case "Created":
                    message = "Hi " + name + "! Your appointment has been scheduled for " + aptDate.ToShortDateString() + " at " + aptTime + ". See you then!";
                    break;
                case "Changed":
                    message = "Hi " + name + "! Your appointment has been changed to " + aptDate.ToShortDateString() + " at " + aptTime + ". See you then!";
                    break;
                case "Cancelled":
                    message = "Hi " + name + "! Your appointment has been cancelled.";
                    break;
                case "Approved":
                    message = "Hi " + name + "! Your appointment has been approved for " + aptDate.ToShortDateString() + " at " + aptTime + ". See you then!";
                    break;
                default:
                    break;
            }
            
            MailMessage mail = new MailMessage();

            mail.To.Add(userEmail);
            mail.From = new MailAddress("shfsgames@gmail.com", "SHFS Admin", System.Text.Encoding.UTF8);
            mail.Subject = "GBA Motors - Appointment " + type;
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
