using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RegisterAndLogin.Models
{
    public class UserAccount
    {
        public User ShowUser{get;set;}
        public Transaction newTrans{get;set;}
        public string ShownBalance{get;set;}
        [Required]
        [Display(Name = "Deposit/Withdraw:")]
        public decimal Amount { get; set; }
    }
}