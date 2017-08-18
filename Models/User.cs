using System;
using System.Collections.Generic;

namespace Auctions.Models
{
    public class User
    {
       public int UserID {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public string Username {get; set;}
        public string Password {get; set;}
        public decimal Wallet {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
        public List<Bid> Bids {get; set;}

        public User(){
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Bids = new List<Bid>();
        }
    }
}