using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RegisterAndLogin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace RegisterAndLogin.Controllers
{
    public class HomeController : Controller
    {
        private UserContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(UserContext context)
        {
            dbContext = context;
        }
        
        
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(User newUser)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            else
            {
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                    Loginuser.SetLogin(HttpContext, newUser.UserId);
                    return Redirect("accout/"+newUser.UserId);
                    // return View("Success");
                }
            }
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(Loginuser newLoginUser)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            else
            {
                User needLogin = dbContext.Users.FirstOrDefault(u => u.Email == newLoginUser.LogEmail);
                if (needLogin == null)
                {
                    ModelState.AddModelError("LogEmail", "This email didn't exist.Please rigester first!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                var verifyPass = Hasher.VerifyHashedPassword(needLogin, needLogin.Password, newLoginUser.LogPassword);
                if (verifyPass == 0)
                {
                    ModelState.AddModelError("LogPassword", "Password is wrong!");
                    return View("Index");
                }
                else
                {
                    Loginuser.SetLogin(HttpContext , needLogin.UserId);
                    return Redirect("accout/" + needLogin.UserId);
                }
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [Route("LogedIn")]
        [HttpGet]
        public IActionResult LogedIn()
        {
            int logedUserID = Loginuser.GetUserID(HttpContext);
            if(logedUserID!=0)
            {
                ViewBag.message = "Welcome";
            }
            else{
                ViewBag.message = "You did not login yet!!!! Can not access this page!";
            }
            return View("LogedIn");
        }
        [Route("accout/{userId}")]
        [HttpGet]
        public IActionResult ShowUserAccount(int userId)
        {
            int logedUserID = Loginuser.GetUserID(HttpContext);
            if (logedUserID == userId)
            {
                User UserwithTransactions = dbContext.Users
                .Include(user => user.Transactions)
                .FirstOrDefault(user => user.UserId == userId);

                decimal currentBalane = dbContext.Users
                    .Include(user => user.Transactions)
                    .FirstOrDefault(user => user.UserId == userId)
                    .Transactions.Sum(trans => trans.Amount); 
                UserAccount newUserAcount = new UserAccount();
                newUserAcount.ShowUser = UserwithTransactions;
                newUserAcount.ShownBalance = currentBalane.ToString("C2");
                return View("UserAccount",newUserAcount);
            }
            else
            {
                ViewBag.message = "You Can not access this page!!!!!!";
                return View("Warning");
            }
            
        }


        [Route("OprateAccount")]
        [HttpPost]
        public IActionResult OprateAccount(UserAccount newUserAcount)
        {
            int logedUserID = Loginuser.GetUserID(HttpContext);
            User UserwithTransactions = dbContext.Users
            .Include(user => user.Transactions)
            .FirstOrDefault(user => user.UserId == logedUserID);
            decimal currentBalane = dbContext.Users
                    .Include(user => user.Transactions)
                    .FirstOrDefault(user => user.UserId == logedUserID)
                    .Transactions.Sum(trans => trans.Amount);
            UserAccount thisUserAcount = new UserAccount();
            thisUserAcount.ShowUser = UserwithTransactions;
            thisUserAcount.ShownBalance = currentBalane.ToString("C2");
            
            if (!ModelState.IsValid)
            {
                return View("UserAccount", thisUserAcount);
            }
            else
            {   
                if(newUserAcount.Amount==0){
                    ModelState.AddModelError("Amount", "Can not deposit/withdaw $0");
                    return View("UserAccount", thisUserAcount);
                }
                else if((0-newUserAcount.Amount)> currentBalane)
                {
                    ModelState.AddModelError("Amount", "Can not withdaw more then your balance!");
                    return View("UserAccount", thisUserAcount);
                }
                else{
                    Transaction newTrans = new Transaction();
                    newTrans.UserId = logedUserID;
                    newTrans.Amount = newUserAcount.Amount;
                    dbContext.Transactions.Add(newTrans);
                    dbContext.SaveChanges();
                    return Redirect("/accout/" + logedUserID);
                }
                    
                    
            }
        }

    }
}
