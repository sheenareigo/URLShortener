using System.ComponentModel.DataAnnotations;

namespace Shortly.Client.Data.ViewModels
{
    public class ConfirmEmailVM
    {
        [Required(ErrorMessage = "Enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Invalid email address")]
        public string emailaddress { get; set; }
    }
}
