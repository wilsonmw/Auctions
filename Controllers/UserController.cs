using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Auctions.Models;
using System.Linq;

namespace Auctions.Controllers
{
    public class UserController : Controller
    {
        private AuctionsContext _context;
 
        public UserController(AuctionsContext context)
        {
            _context = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.existsError = HttpContext.Session.GetString("existsError");
            ViewBag.loginError = HttpContext.Session.GetString("loginError");
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model){
            if(!ModelState.IsValid){
                return View("Index");
            }
            else{
                bool exists = _context.Users.Any(u => u.Username == model.Username);
                if (exists == true){
                    HttpContext.Session.SetString("existsError", "That username is already in use, please try again.");
                    return RedirectToAction ("Index");
                }
                else
                {
                    User newUser = new User{
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Password = model.Password,
                        Wallet = 1000.00m,
                    };
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    User currentUser = _context.Users.Single(u => u.Username == newUser.Username);
                    HttpContext.Session.SetInt32("userID", currentUser.UserID);
                    return RedirectToAction("AllAuctions", "Auction");
                } 
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string Username, string Password){
            if (Username.Length < 1 || Password.Length < 1){
                HttpContext.Session.SetString("loginError", "Login failed, please try again.");
                return RedirectToAction("Index");
            }
            else{
                bool exists = _context.Users.Any(u => u.Username == Username);
                if(exists == true){
                    User currentUser = _context.Users.Single(u => u.Username == Username);
                    if (currentUser.Password == Password){
                        HttpContext.Session.SetInt32("userID", currentUser.UserID);
                        return RedirectToAction("AllAuctions", "Auction");
                    }
                    else{
                        HttpContext.Session.SetString("loginError", "Login failed, please try again.");
                        return RedirectToAction("Index");
                    }
                }
                else{
                    HttpContext.Session.SetString("loginError", "Login failed, please try again.");
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "User");
        }


    }
}
