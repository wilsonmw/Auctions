using System;

namespace Auctions.Models
{
    public class Bid
    {
        public int BidID {get; set;}
        public decimal Amount {get; set;}
        public int UserID {get; set;}
        public User User {get; set;}
        public int AuctionID {get; set;}
        public Auction Auction {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
        
    }
}