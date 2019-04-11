using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace RegisterAndLogin.Models
{
    public class Loginuser
    {
        [Required(ErrorMessage = "An Email is required")]
        [EmailAddress(ErrorMessage = "A Valid Email is required")]
        [Display(Name = "Your registed email:")]
        public string LogEmail { get; set; }
        [Required(ErrorMessage = "A Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password Must be 8 characters or longer!")]
        [Display(Name = "Your registed password:")]
        public string LogPassword { get; set; }

        

        public static void SetLogin(HttpContext context, int id)
        {
            context.Session.SetString("UserID", id.ToString());
        }
        public static int GetUserID(HttpContext context)
        {
            string Id = context.Session.GetString("UserID");
            int UserId = Convert.ToInt32(Id);
            return UserId;
        }
    }
}