using System.ComponentModel.DataAnnotations;

namespace Shortly.Client.Data.ViewModels
{
    public class PostUrlVM
    {
        [Required(ErrorMessage = "URL is required")]
        [RegularExpression(@"^(https?://)?([\da-z\.-]+)\.([a-z\.]{2,6})([/\w \.-]*)*/?$", ErrorMessage = "The value is not a valid URL")]
        public string Url { get; set; }
    }
}
