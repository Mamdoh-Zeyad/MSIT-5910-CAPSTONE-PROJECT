using System.ComponentModel.DataAnnotations;

namespace DevOpsMetricsApp.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "System Role")]
        public string Role { get; set; }
    }
}