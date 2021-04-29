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

namespace GBA_Application.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly GBAdbContext _context;

        public EmployeesController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                return View(await _context.Employees.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToListAsync());

            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Employees Home Page
        public async Task<IActionResult> EmployeeHomePage()
        {
            if (HttpContext.Session.GetString("employeeId") != null)
            {
                return View("EmployeeHomePage");

            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employees = await _context.Employees
                    .FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employees == null)
                {
                    return NotFound();
                }

                return View(employees);
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        //GET: Employees/Login
        public IActionResult EmployeeLogin()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("employeeId")) && String.IsNullOrEmpty(HttpContext.Session.GetString("memberId")))
            {
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //POST: Employee/Login
        [HttpPost]
        public ActionResult EmployeeLogin(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var employees = _context.Employees.Where(r => r.Email.Equals(email) && r.Password.Equals(password));

                if (employees.Count() == 1)
                {
                    //check employee ID. If ID == 1, then admin else other employee
                    if (HttpContext.Session.GetString("userType") == null || HttpContext.Session.GetString("userType") == "")
                    {
                        var empId = employees.FirstOrDefault().EmployeeId;

                        if (empId == 1)
                        {
                            //ADMIN
                            HttpContext.Session.SetString("userType", "admin");

                            if (HttpContext.Session.GetString("employeeId") == null || HttpContext.Session.GetString("employeeId") == "")
                            {
                                HttpContext.Session.SetString("employeeId", employees.FirstOrDefault().EmployeeId.ToString());

                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetString("userType", "employee");

                            if (HttpContext.Session.GetString("employeeId") == null || HttpContext.Session.GetString("employeeId") == "")
                            {
                                HttpContext.Session.SetString("employeeId", employees.FirstOrDefault().EmployeeId.ToString());

                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.error = "Please Try Again";
                }
            }

            return View();
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Email,Password,FirstName,LastName")] Employees employees, string Password2)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (ModelState.IsValid)
                {
                    //validate email and password
                    var emailValid = UniqueEmail(employees.Email);
                    var passwordValid = ValidatePassword(employees.Password);

                    if (String.IsNullOrEmpty(employees.Password) || String.IsNullOrEmpty(Password2))
                    {
                        passwordValid = false;
                    }

                    var passwordsMatch = false;

                    if (employees.Password == Password2)
                    {
                        passwordsMatch = true;
                    }

                    if (emailValid && passwordValid && passwordsMatch)
                    {
                        _context.Add(employees);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (!emailValid)
                        {
                            ViewBag.errorEmail = "An account with this email already exists";
                        }
                        if (!passwordValid)
                        {
                            ViewBag.errorPassword = "Password must contain a capital letter and number, and min 8 chars";
                        }
                        if (!passwordsMatch)
                        {
                            ViewBag.errorPasswordConfirm = "Passwords don't match";
                        }
                    }
                }
                return View(employees);
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employees = await _context.Employees.FindAsync(id);
                if (employees == null)
                {
                    return NotFound();
                }
                return View(employees);
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Email,Password,FirstName,LastName")] Employees employees)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (id != employees.EmployeeId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var employee = _context.Employees.Where(a => a.EmployeeId == id).AsNoTracking().FirstOrDefault();
                    employees.Password = employee.Password;

                    var oldEmail = employee.Email;
                    var emailValid = UniqueEmail(employees.Email, oldEmail);

                    if (emailValid)
                    {
                        try
                        {
                            _context.Update(employees);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!EmployeesExists(employees.EmployeeId))
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
                        if (!emailValid)
                        {
                            ViewBag.errorEmail = "An account with this email already exists";
                        }
                    }
                }
                return View(employees);
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employees = await _context.Employees
                    .FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employees == null)
                {
                    return NotFound();
                }

                return View(employees);
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("userType") == "admin")
            {
                if (id != 1)
                {
                    var employees = await _context.Employees.FindAsync(id);
                    _context.Employees.Remove(employees);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.error = "Can't delete admin account";
                    return RedirectToAction("Index");
                }
            }
            else if (HttpContext.Session.GetString("userType") == "employee")
            {
                return View("EmployeeHomePage");
            }
            else
            {
                return RedirectToAction("Login", "Members");
            }
        }

        //GET: Employees/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //POST: Employees/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var employee = _context.Employees.Where(a => a.Email.ToLower().Equals(email.ToLower())).FirstOrDefault();

                if (employee != null)
                {
                    //change their password
                    var newPassword = PasswordGenerator();

                    employee.Password = newPassword;
                    try
                    {
                        _context.Update(employee);
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }

                    //email new password
                    var empEmail = employee.Email;

                    MailMessage mail = new MailMessage();

                    mail.To.Add(empEmail);
                    mail.From = new MailAddress("shfsgames@gmail.com", "SHFS Admin", System.Text.Encoding.UTF8);
                    mail.Subject = "Password Reset";
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = "Your new password is: " + newPassword;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("shfsgames@gmail.com", "ShfsGames687.");
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(mail);
                        return View(nameof(EmployeeLogin));
                    }
                    catch (Exception e)
                    {
                        ViewBag.error = e.Message;
                    }
                }
                else
                {
                    ViewBag.error = "An account with this email does not exist";
                }
            }

            return View();
        }

        //GET: Employee/Change Password
        public IActionResult ChangePassword()
        {
            return View();
        }

        //POST: Employee/Change Password
        [HttpPost]
        public ActionResult ChangePassword(string password, string newPassword1, string newPassword2)
        {
            if (ModelState.IsValid)
            {
                //check if old password matches
                int empId = int.Parse(HttpContext.Session.GetString("employeeId"));

                var employee = _context.Employees.Where(a => a.EmployeeId.Equals(empId)).FirstOrDefault();

                if (employee != null)
                {
                    var empOldPass = employee.Password;

                    if (password == empOldPass)
                    {
                        //check if new password 1 and 2 match
                        if (newPassword1 == newPassword2)
                        {
                            //check if old password is the same as new password
                            if (password != newPassword1)
                            {
                                //validate new password
                                var validatePassword = ValidatePassword(newPassword1);

                                if (validatePassword)
                                {
                                    //change their password
                                    employee.Password = newPassword1;
                                    try
                                    {
                                        _context.Update(employee);
                                        _context.SaveChanges();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        throw;
                                    }
                                    return View("EmployeeHomePage");
                                }
                                else
                                {
                                    ViewBag.error = "Password must contain a capital letter and number, and min 8 chars";
                                }
                            }
                            else
                            {
                                ViewBag.error = "New password cannot be the same as old password";
                            }
                        }
                        else
                        {
                            ViewBag.error = "New passwords do not match";
                        }
                    }
                    else
                    {
                        ViewBag.error = "Old Password is incorrect";
                    }
                }
                else
                {
                    ViewBag.error = "Login first";
                }
            }

            return View();
        }

        private bool EmployeesExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        /// <summary>
        /// Method to check if the email already exists or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UniqueEmail(string email)
        {
            email = email.ToLower();

            var users = _context.Members.Where(r => r.Email.ToLower().Equals(email));

            if (users.Count() == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Check for unique email when editing
        /// </summary>
        /// <param name="email"></param>
        /// <param name="oldEmail"></param>
        /// <returns></returns>
        public bool UniqueEmail(string email, string oldEmail)
        {
            email = email.ToLower();

            var users = _context.Members.Where(r => r.Email.ToLower().Equals(email));

            if (users.Count() == 1)
            {
                //check if it's the old email
                if (email == oldEmail)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Validate if password is a strong password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidatePassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasAnUpper = new Regex(@"[A-Z]+");
            var min8Chars = new Regex(@".{8,}");

            if (hasNumber.IsMatch(password) && hasAnUpper.IsMatch(password) && min8Chars.IsMatch(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string PasswordGenerator()
        {
            char[] password = new char[10];

            string passwordChar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                password[i] = passwordChar[random.Next(passwordChar.Length - 1)];
            }

            return string.Join(null, password);
        }
    }
}
