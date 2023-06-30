using System.ComponentModel.DataAnnotations;

namespace MessageNowApi.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(32, ErrorMessage = "The {0} must be at least {2} and not more than {1} characters long.", MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and not more than {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
