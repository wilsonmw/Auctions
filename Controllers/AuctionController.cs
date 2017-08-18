using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Auctions.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Controllers
{
    public class AuctionController : Controller
    {
        private AuctionsContext _context;
 
        public AuctionController(AuctionsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("allAuctions")]
        public IActionResult AllAuctions(){
            int? loggedIn = HttpContext.Session.GetInt32("userID");
            if(loggedIn == null){
                return RedirectToAction("Index", "User");
            }
            User currentUser = _context.Users.Include(b => b.Bids).Single(u => u.UserID == loggedIn);
            ViewBag.currentUser = currentUser;
            List<Auction> AllAuctions = _context.Auctions.Include(b => b.Bids).Include(u => u.User).ToList();
            ViewBag.AllAuctions = AllAuctions;
            foreach (var auction in AllAuctions){
                int NewDaysLeft = (auction.EndDate - DateTime.Now).Days;
                auction.DaysLeft = NewDaysLeft;
                if(auction.DaysLeft <= 0){
                    foreach (var bid in auction.Bids){
                        if(bid.Amount == auction.TopBid){
                            User PaysMoney = _context.Users.Single(u => u.UserID == bid.UserID);
                            PaysMoney.Wallet = PaysMoney.Wallet - auction.TopBid;
                            auction.User.Wallet = auction.User.Wallet + auction.TopBid;
                            _context.SaveChanges();
                        }
                    }
                    _context.Auctions.Remove(auction);   
                }
                _context.SaveChanges();
            }
            


            return View("AllAuctions");
        }

        [HttpGet]
        [Route("create")]
        public IActionResult CreateNew(){
            int? loggedIn = HttpContext.Session.GetInt32("userID");
            if(loggedIn == null){
                return RedirectToAction("Index", "User");
            }
            return View("NewAuction");
        }

        [HttpPost]
        [Route("new")]
        public IActionResult NewAuction(AuctionViewModel model){
            int? loggedIn = HttpContext.Session.GetInt32("userID");
            if(loggedIn == null){
                return RedirectToAction("Index", "User");
            }
            if(ModelState.IsValid){
                Auction newAuction = new Auction{
                    Product = model.Product,
                    Description = model.Description,
                    StartingBid = model.StartingBid,
                    EndDate = model.EndDate,
                    UserID = (int)loggedIn,
                    DaysLeft = (model.EndDate - DateTime.Now).Days
                };
                _context.Auctions.Add(newAuction);
                _context.SaveChanges();
                return RedirectToAction("AllAuctions");
            }
            else{
                return View("NewAuction");
            }
        }

        [HttpGet]
        [Route("product/{id}")]
        public IActionResult ViewAuction(int id){
            int? loggedIn = HttpContext.Session.GetInt32("userID");
            if(loggedIn == null){
                return RedirectToAction("Index", "User");
            }
            Auction currentAuction = _context.Auctions.Single(i => i.AuctionID == id);
            User currentUser = _context.Users.Include(b => b.Bids).Single(u => u.UserID == loggedIn);
            ViewBag.currentUser = currentUser;
            ViewBag.currentAuction = currentAuction;
            ViewBag.Owner = _context.Users.Single(i => i.UserID == currentAuction.UserID);
            bool exists = _context.Bids.Any(u => u.AuctionID == currentAuction.AuctionID);
            if(exists == true){ 
                Bid currentTopBid = _context.Bids.Single(b => b.Amount == currentAuction.TopBid);
                ViewBag.currentTopBid = currentTopBid;
                User TopBidder = _context.Users.Single(u => u.UserID == currentTopBid.UserID);
                ViewBag.TopBidder = TopBidder.Username;
            }
            else{
                ViewBag.TopBidder = "No bids made yet!";
            }
            return View("singleView");
        }

        [HttpPost]
        [Route("newBid")]
        public IActionResult NewBid(decimal bid, int userID, int productID){
            User currentUser = _context.Users.Single(u =>u.UserID == userID);
            Auction currentAuction = _context.Auctions.Single(i => i.AuctionID == productID);
            if (bid > currentUser.Wallet){
                HttpContext.Session.SetString("bidError", "You do not have enough money to bid that much.");
                return RedirectToAction("AllAuctions");
            }
            else if (bid < currentAuction.TopBid){
                HttpContext.Session.SetString("bidError", "You must bid more than the current top bid.");
                return RedirectToAction("AllAuctions");
            }
            else if (bid < currentAuction.StartingBid){
                HttpContext.Session.SetString("bidError", "You must bid more than the starting bid.");
                return RedirectToAction("AllAuctions");
            }
            else{
                Bid newBid = new Bid{
                    Amount = bid,
                    UserID = userID,
                    AuctionID = productID
                };
                _context.Bids.Add(newBid);
                currentAuction.TopBid = bid;
                _context.SaveChanges();
                return RedirectToAction("AllAuctions");
            }
           
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id){
            Auction currentAuction = _context.Auctions.Single(i => i.AuctionID == id);
            _context.Auctions.Remove(currentAuction);
            _context.SaveChanges();
            return RedirectToAction("AllAuctions");
        }

    }
}