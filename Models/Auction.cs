using System;
using System.Collections.Generic;

namespace Auctions.Models
{
    public class Auction
    {
        public int AuctionID {get; set;}
        public string Product {get; set;}
        public string Description {get; set;}
        public decimal StartingBid {get; set;}
        public DateTime EndDate {get; set;}
        public int UserID {get; set;}
        public User User {get; set;}
        public int DaysLeft {get; set;}
        public decimal TopBid {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
        public List<Bid> Bids {get; set;}

        public Auction(){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            TopBid = 0;
            Bids = new List<Bid>();
        }
    }
}