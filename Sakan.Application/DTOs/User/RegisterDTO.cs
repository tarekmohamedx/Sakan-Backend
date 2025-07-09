using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Application.DTOs.User
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; }
        [Required]
        [StringLength(100,MinimumLength =6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage ="Phone number is required")]
        [Phone(ErrorMessage ="invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}
