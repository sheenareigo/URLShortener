using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Shortly.Client.Data.ViewModels
{
    public class LoginVM
    {

        [Required(ErrorMessage ="Enter email address")]
        [EmailAddress(ErrorMessage ="Invalid email address")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage ="Invalid email address")]
    public string emailaddress { get; set; }

        [Required(ErrorMessage ="Enter password")]
        [MinLength(5,ErrorMessage ="Password must be atleast 5 characters")]
    public string password { get; set; }

        public IEnumerable<AuthenticationScheme> ?schemes { get; set; }
    }

}
