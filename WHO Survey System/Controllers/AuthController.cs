using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WHO_Survey_System.BL;
using WHO_Survey_System.HelpingClasses;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly GeneralPurpose gp;
        private readonly SqlConnection de = new SqlConnection();
        private readonly DatabaseEntities db = new DatabaseEntities();

        public AuthController()
        {
            this.gp = new GeneralPurpose();
          
            this.de = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString.Replace("metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=\"", "").Replace("\"", ""));
        }

        #region Log IN      
        public ActionResult Login(string msg = "", string color = "")
        {
            int userCount = new UserBL().GetActiveUserList(de).Count();
            if (userCount == 0)
            {
                User obj = new User()
                {
                    FirstName = "Uzair",
                    LastName = "Aslam",
                    Email = "uzair.aslam02@gmail.com",
                    Password = StringCipher.Encrypt("123"),
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = DateTime.Now,
                };
                User obj2 = new User()
                {
                    FirstName = "Darren",
                    LastName = "Greeno",
                    Email = "darren@deiready.com",
                    Password = StringCipher.Encrypt("123"),
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = DateTime.Now,
                };
                new UserBL().AddUser(obj, de);
                new UserBL().AddUser(obj2, de);
            }

            if (gp.ValidateLoggedinUser() != null)
            {
                if (gp.ValidateLoggedinUser().Role == 1 /*|| gp.ValidateLoggedinUser().Role == 2*/)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //else if (gp.ValidateLoggedinUser().Role == 3)
                //{
                //    return RedirectToAction("Index", "Home");
                //}
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        public ActionResult PostLogin(string Email = "", string Password = "")
        {
            try
            {
                User user = new UserBL().GetActiveUserList(de).Where(x => x.Email.ToLower() == Email.ToLower() && StringCipher.Decrypt(x.Password).Equals(Password)).FirstOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
                }
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim("UserName", user.FirstName + user.LastName ),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Role", user.Role.ToString())
                }, "ApplicationCookie");

                var claimsPrincipal = new ClaimsPrincipal(identity); // Set current principal
                Thread.CurrentPrincipal = claimsPrincipal;
                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                if (user.Role == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    //return RedirectToAction("Index", "Home");
                    return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Sign up
        public ActionResult Register(string msg = "", string color = "black")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }
        public ActionResult PostRegister(User _user, string ConfirmPassword = "")
        {
            try
            {
                if (_user.Password != ConfirmPassword)
                {
                    return RedirectToAction("Register", "Auth", new { msg = "Password and confirm password didn't match", color = "red" });
                }
                bool checkEmail = gp.ValidateEmail(_user.Email);
                if (checkEmail == false)
                {
                    return RedirectToAction("Register", "Auth", new { msg = "Email already exist. Try another!", color = "red" });
                }

                User u = new User()
                {
                    FirstName = _user.FirstName.Trim(),
                    LastName = _user.LastName.Trim(),
                    Email = _user.Email.Trim(),
                    Password = StringCipher.Encrypt(_user.Password.Trim()),
                    Role = 2,
                    IsActive = 1,
                    CreatedAt = DateTime.Now
                };

                bool checkUser = new UserBL().AddUser(u, de);

                if (checkUser == true)
                {
                    return RedirectToAction("Login", new { msg = "Signup successfull, Try login", color = "green" });
                }
                else
                {
                    return RedirectToAction("Register", "Auth", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Forgot Password
        public ActionResult ForgotPassword(string msg = "", string color = "black")
        {
            ViewBag.Color = color;
            ViewBag.Message = msg;

            return View();
        }
        public ActionResult PostForgotPassword(string Email = "")
        {
            try
            {
                bool checkEmail = gp.ValidateEmail(Email);

                if (checkEmail == false)
                {
                    string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");

                    bool checkMail = MailSender.SendForgotPasswordEmail(Email, BaseUrl);

                    if (checkMail == true)
                    {
                        return RedirectToAction("Login", "Auth", new { msg = "Please check your inbox/spam", color = "green" });
                    }
                    else
                    {
                        return RedirectToAction("ForgotPassword", "Auth", new { msg = "Mail sending fail!", color = "red" });
                    }
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Email does not belong to our record!!", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ResetPassword(string email = "", string time = "", string msg = "", string color = "black")
        {
            try
            {
                DateTime PassDate = Convert.ToDateTime(StringCipher.Base64Decode(time)).Date;
                DateTime CurrentDate = DateTime.Now.Date;

                if (CurrentDate != PassDate)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Link expired, Please try again!", color = "red" });
                }

                ViewBag.Time = time;
                ViewBag.Email = email;
                ViewBag.Message = msg;
                ViewBag.Color = color;

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        public ActionResult PostResetPassword(string Email = "", string Time = "", string NewPassword = "", string ConfirmPassword = "")
        {
            try
            {
                if (NewPassword != ConfirmPassword)
                {
                    return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Password and confirm password did not match", color = "red" });
                }

                string DecryptEmail = StringCipher.Base64Decode(Email);

                User user = new UserBL().GetActiveUserList(de).Where(x => x.Email.Trim().ToLower() == DecryptEmail.Trim().ToLower()).FirstOrDefault();

                user.Password = StringCipher.Encrypt(NewPassword);

                bool check = new UserBL().UpdateUser(user, de);

                if (check == true)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Password reset successful, Try Login", color = "green" });
                }
                else
                {
                    return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Somethings' Wrong!", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Manage Profile
        public ActionResult UpdateProfile(string msg = "", string color = "black")
        {
            if (gp.ValidateLoggedinUser() == null)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }
        public ActionResult PostUpdateProfile(User _user)
        {
            try
            {
                if (gp.ValidateLoggedinUser() == null)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                bool chkEmail = gp.ValidateEmail(_user.Email, _user.Id);

                if (chkEmail == false)
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { msg = "Email used by someone else, Please try another", color = "red" });
                }

                User u = new UserBL().GetUserById(_user.Id, de);
                u.FirstName = _user.FirstName.Trim();
                u.LastName = _user.LastName.Trim();
                u.Email = _user.Email.Trim();
                u.Password = StringCipher.Encrypt(_user.Password.Trim());

                bool chkUser = new UserBL().UpdateUser(u, de);

                if (chkUser == true)
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { msg = "Profile updated successfully!", color = "green" });
                }
                else
                {
                    return RedirectToAction("UpdateProfile", "Auth", new { msg = "Somthings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }
        #endregion
    }
}