using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Shortly.Client.Data.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "User name is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "Enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Invalid email address")]
        public string emailaddress { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        public string password { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string confirmpassword { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\+1\d{10}$", ErrorMessage = "Phone number must be a valid Canadian number with 10 digits following the country code +1")]
        public string phonenumber { get; set; }

        public IEnumerable<AuthenticationScheme>? schemes { get; set; }


    }
}

   
