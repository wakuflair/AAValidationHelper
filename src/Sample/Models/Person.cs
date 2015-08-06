using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class Person
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [MinLength(2, ErrorMessage = "{0} must be more than {1} letters.")]
        [MaxLength(6, ErrorMessage = "{0} must be less than {1} letters.")]
        public string Name { get; set; }

        [DisplayName("Age")]
        [Required(ErrorMessage = "{0} cannot be empty")]
        [Range(1, 150, ErrorMessage = "Please input valid {0}.({1}-{2})")]
        public int Age { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress(ErrorMessage = "Please input valid {0}.")]
        public string EmailAddress { get; set; }

        [DisplayName("ID")]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "{0} must be composed of 1-4 digits")]
        public string Id { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Compare("Password", ErrorMessage = "{0} must be the same as the Password.")]
        public string ConfirmPassword { get; set; }
    }
}