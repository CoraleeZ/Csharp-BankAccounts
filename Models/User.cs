using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace RegisterAndLogin.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password Must be 8 characters or longer!")]
        [Display(Name = "Password:")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [CompareAttribute("Password", ErrorMessage = "Passwords Must Match!")]
        [NotMapped]
        [Display(Name = "Confirm your password:")]
        public string PasswordConfirm { get; set; }

        public List<Transaction> Transactions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        [Display(Name = "Deposit/Withdraw:")]
        public decimal Amount { get; set; } 
        public int UserId { get; set; }
        // Navigation property for related User object
        public User Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
       
    }

}