using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBA_Application.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;

namespace GBA_Application.Controllers
{
    public class MembersController : Controller
    {
        private readonly GBAdbContext _context;

        public MembersController(GBAdbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.OrderBy(a => a.FirstName).ToListAsync());
        }

        // GET: Members
        public async Task<IActionResult> MemberHomePage()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                return View();
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return View("Login");
            }
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details()
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                int memberId = int.Parse(HttpContext.Session.GetString("memberId"));

                if (memberId == null)
                {
                    return NotFound();
                }

                var members = await _context.Members
                    .FirstOrDefaultAsync(m => m.MemberId == memberId);
                if (members == null)
                {
                    return NotFound();
                }

                return View(members);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return View("Login");
            }
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FirstName,LastName,Email,Phone,Password,DateOfBirth,Address,PostalCode,Province,City,Country")] Members members, string Password2)
        {
            if (ModelState.IsValid)
            {
                //validate
                var emailValid = UniqueEmail(members.Email);
                var passwordValid = ValidatePassword(members.Password);
                var postalValid = false;
                if (members.PostalCode != null)
                {
                    postalValid = ValidatePostal(members.PostalCode);
                }
                else
                {
                    postalValid = true;
                }

                if (String.IsNullOrEmpty(members.Password) || String.IsNullOrEmpty(Password2))
                {
                    passwordValid = false;
                }

                var passwordsMatch = false;

                if (members.Password == Password2)
                {
                    passwordsMatch = true;
                }

                if (emailValid && passwordValid && passwordsMatch && postalValid)
                {
                    _context.Add(members);
                    await _context.SaveChangesAsync();

                    //set user type and memberID session variables
                    HttpContext.Session.SetString("userType", "member");
                    HttpContext.Session.SetString("memberId", members.MemberId.ToString());

                    var fname = members.FirstName;
                    var lname = members.LastName;
                    HttpContext.Session.SetString("userFullName", fname + " " + lname);

                    ViewBag.memberName = members.FirstName + " " + members.LastName;

                    return View("MemberHomePage");
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
                    if (!postalValid)
                    {
                        ViewBag.errorPostal = "Enter correct Postal Code Format (X1X1X1)";
                    }
                }
                
            }
            return View(members);
        }

        //GET: Members/Login
        public IActionResult Login()
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

        //POST: Members/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Members.Where(r => r.Email.Equals(email) && r.Password.Equals(password)).FirstOrDefault();

                if (user != null)
                {
                    var id = user.MemberId;
                    //set session type as member
                    //check if someone is already logged in
                    if (HttpContext.Session.GetString("userType") == null || HttpContext.Session.GetString("userType") == "")
                    {
                        HttpContext.Session.SetString("userType", "member");

                        //set session memberId as id, check first if it's null
                        if (HttpContext.Session.GetString("memberId") == null || HttpContext.Session.GetString("memberId") == "")
                        {
                            HttpContext.Session.SetString("memberId", id.ToString());
                            var fname = user.FirstName;
                            var lname = user.LastName;
                            HttpContext.Session.SetString("userFullName", fname + " " + lname);

                            var memberName = _context.Members.Where(a => a.MemberId == id).FirstOrDefault();
                            ViewBag.memberName = memberName.FirstName + " " + memberName.LastName;

                            return View("MemberHomePage");
                        }
                    }
                    else
                    {
                        ViewBag.error = "Error occured, please try again";
                    }
                }
                else
                {
                    ViewBag.error = "Email or Password doesn't exist or is incorrect";
                }
            }

            return View();
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var members = await _context.Members.FindAsync(id);
                if (members == null)
                {
                    return NotFound();
                }
                return View(members);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return View("Login");
            }
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FirstName,LastName,Email,Phone,Password,DateOfBirth,Address,PostalCode,Province,City,Country")] Members members)
        {
            if (HttpContext.Session.GetString("memberId") != null)
            {
                if (id != members.MemberId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    //keep password the same
                    var member = _context.Members.Where(a => a.MemberId == id).AsNoTracking().FirstOrDefault();
                    members.Password = member.Password;

                    var oldEmail = member.Email;
                    var emailValid = UniqueEmail(members.Email, oldEmail);

                    var postalValid = ValidatePostal(members.PostalCode);

                    if (emailValid && postalValid)
                    {
                        try
                        {
                            _context.Update(members);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!MembersExists(members.MemberId))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Details));
                    }
                    else
                    {
                        if (!emailValid)
                        {
                            ViewBag.errorEmail = "An account with this email already exists";
                        }
                        if (!postalValid)
                        {
                            ViewBag.errorPostal = "Enter correct Postal Code Format (X1X1X1)";
                        }
                    }
                }
                return View(members);
            }
            else
            {
                ViewBag.error = "An error occured, login again";
                return View("Login");
            }
        }

        //GET: Members/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //POST: Members/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Members.Where(a => a.Email.ToLower().Equals(email.ToLower())).FirstOrDefault();

                if (user != null)
                {
                    //change their password
                    var newPassword = PasswordGenerator();

                    user.Password = newPassword;
                    try
                    {
                        _context.Update(user);
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }

                    //email new password
                    var userEmail = user.Email;

                    MailMessage mail = new MailMessage();

                    mail.To.Add(userEmail);
                    mail.From = new MailAddress("shfsgames@gmail.com", "SHFS Admin", System.Text.Encoding.UTF8);
                    mail.Subject = "Password Reset";
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    mail.Body = "Your new password is: " + newPassword;
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
                        return View(nameof(Login));
                    }
                    catch (Exception e)
                    {
                        ViewBag.error = e.Message;
                    }
                }
                else
                {
                    ViewBag.error = "An account with this email does not exists";
                }
            }

            return View();
        }

        //GET: Members/Change Password
        public IActionResult ChangePassword()
        {
            return View();
        }

        //POST: Members/Change Password
        [HttpPost]
        public ActionResult ChangePassword(string password, string newPassword1, string newPassword2)
        {
            if (ModelState.IsValid)
            {
                //check if old password matches
                int memberId = int.Parse(HttpContext.Session.GetString("memberId"));

                var member = _context.Members.Where(a => a.MemberId.Equals(memberId)).FirstOrDefault();

                if (member != null)
                {
                    var memberOldPass = member.Password;

                    if (password == memberOldPass)
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
                                    member.Password = newPassword1;
                                    try
                                    {
                                        _context.Update(member);
                                        _context.SaveChanges();
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        throw;
                                    }
                                    return View("MemberHomePage");
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

        private bool MembersExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
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

        public bool ValidatePostal(string postal)
        {
            var matchesCAD = new Regex(@"^[a-z]\d[a-z]\ ?\d[a-z]\d$");

            if (matchesCAD.IsMatch(postal.ToLower()))
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
