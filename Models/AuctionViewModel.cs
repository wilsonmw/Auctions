

using System;
using System.ComponentModel.DataAnnotations;

namespace Auctions.Models
{
    public class AuctionViewModel
    {
        [Required]
        [MinLength(3)]
        public string Product {get; set;}
        [Required]
        [MinLength(10)]
        public string Description {get; set;}
        [Required]
        [Bid(ErrorMessage="Starting Bid must be more than $0.00")]
        [Display(Name="Starting Bid")]
        public decimal StartingBid {get; set;}
        [Required]
        [FutureDate(ErrorMessage="End Date cannot be in the past")]
        [Display(Name="Auction End Date")]
        public DateTime EndDate {get; set;}


    }

    public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime theirDate = Convert.ToDateTime(value);
                return theirDate >= DateTime.Now;
            }
        }  

    public class BidAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                decimal startBid = Convert.ToDecimal(value);
                if(startBid <=0){
                    return false;
                }
                else{
                    return true;
                }
            }
        }
}