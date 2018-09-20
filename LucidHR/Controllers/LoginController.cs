using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using LucidHR.Models;
using System.Web.Helpers;
namespace LucidHR.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login

        //Start account mail funcs
        private bool SendForgetEmail(string email, string token)
        {
            var body = "<p>Please click this link to change your password: <a href='http://localhost:63599/login/Reset?token=" + token + "'>Click</a></p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));  // replace with valid value 
            message.From = new MailAddress("no-reply@ukraine.az");  // replace with valid value
            message.Subject = "Sifr? sifirlamasi";
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "elcnaghyv@gmail.com",  // replace with valid value
                    Password = "elcin238"  // replace with valid value-Detiva-Password-@pww({Kf2wfy@AL7
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                //smtp.Host = "smtp.live.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(message);
                return true;
            }
        }
        //End account mail funcs

        LucidEntities db = new LucidEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string email, string password)
        {
            //null data control
            if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password))
            {
                Session["loginError"] = "Please fill inputs correctly";
                return RedirectToAction("index");
            }

            //email existing control
            if (db.Users.FirstOrDefault(u => u.Email == email) == null)
            {
                Session["loginError"] = "This employee is not exist";
                return RedirectToAction("index");
            }

            if (Crypto.VerifyHashedPassword(db.Users.FirstOrDefault(u => u.Email == email).Password, password))
            {
                Employee emp = db.Employees.Find(db.Users.FirstOrDefault(u => u.Email == email).EmployeeId);
                Session["login"] = new Employee();
                Session["login"] = emp;
                return RedirectToAction("index", "home");
            }
            Session["loginError"] = "Please fill inputs correctly";
            return RedirectToAction("index");

        }

        public ActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot(string email)
        {
            if (!String.IsNullOrWhiteSpace(email))
            {
                if (db.Users.FirstOrDefault(u => u.Email == email) == null)
                {
                    Session["forgotError"] = "This account is not exist";
                    return RedirectToAction("forgot");
                }
                User usr = db.Users.FirstOrDefault(u => u.Email == email);
                usr.Token = Crypto.Hash(usr.Email + DateTime.Now.ToString("yyyyMMddHHmmss"), "sha256");
                usr.IsConfirm = false;
                db.SaveChanges();
                SendForgetEmail(usr.Email, usr.Token);
                Session["SendForgot"] = "We send email to you for resetting your password";
                return RedirectToAction("index");
            }
            Session["forgotError"] = "Please fill inputs correctly";
            return RedirectToAction("forgot");
        }

        public ActionResult Reset(string token)
        {
            if (!String.IsNullOrWhiteSpace(token))
            {
                if (db.Users.FirstOrDefault(u => u.Token == token) != null && db.Users.FirstOrDefault(u => u.Token == token).IsConfirm != true)
                {
                    Session["resetUser"] = db.Users.FirstOrDefault(u => u.Token == token);

                    db.Users.FirstOrDefault(u => u.Token == token).IsConfirm = true;
                    db.Users.FirstOrDefault(u => u.Token == token).Token = "";
                    db.SaveChanges();
                    Session["confirmForgot"] = true;
                    return RedirectToAction("forgot",new {});
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Reset(string password, string confirmPassword)
        {
            //if (String.IsNullOrWhiteSpace(passsword) || String.IsNullOrWhiteSpace(confirmPassword))
            //{
            //    return HttpNotFound();
            //}   
            User usr = (User)Session["resetUser"];
            if (password == confirmPassword)
            {
                db.Users.Find(usr.Id).Password = Crypto.HashPassword(password);
                db.SaveChanges();
                Session["resetPassword"] = "You changed your password";
                return RedirectToAction("index");
            }
            return Content(password);
        }

        //register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User usr,string confirmPassword,string Name,string Surname)
        {
            if(String.IsNullOrWhiteSpace(usr.Email)|| String.IsNullOrWhiteSpace(usr.Password) || String.IsNullOrWhiteSpace(confirmPassword) 
                || String.IsNullOrWhiteSpace(Name) || String.IsNullOrWhiteSpace(Surname))
            {
                Session["registerError"] = "Please fill inputs correctly";
                return RedirectToAction("register");
            }
            if (usr.Password != confirmPassword)
            {
                Session["registerError"] = "Your passwords are not same";
                return RedirectToAction("register");
            }
            if (db.Users.FirstOrDefault(u => u.Email == usr.Email) != null)
            {
                Session["registerError"] = "This account already exist";
                return RedirectToAction("register");
            }
            usr.Password = Crypto.HashPassword(usr.Password);
            usr.EmployeeId = db.Employees.FirstOrDefault(e => e.Email == usr.Email).Id;
            db.Users.Add(usr);
            db.SaveChanges();
            Session["login"] = new Employee();
            Session["login"] = db.Employees.FirstOrDefault(e=>e.Email==usr.Email);
            return RedirectToAction("index", "home");
        }
    }
}