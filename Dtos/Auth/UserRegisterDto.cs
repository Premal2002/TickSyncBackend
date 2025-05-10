using System.ComponentModel.DataAnnotations;

namespace TickSyncAPI.Dtos.Auth
{
    public class UserRegisterDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "FullName field is required")]
        [MinLength(3, ErrorMessage = "Name must be atleast 3 characters long")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage = "Enter a valid Email Id")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone field is required")]
        [RegularExpression("^[7-9][0-9]{9}$", ErrorMessage = "Enter a valid 10-digit phone number starting with 7, 8, or 9.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Password field is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, lowercase letter, number, and special character.")]
        public string PasswordHash { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
    }
}
