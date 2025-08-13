using System.ComponentModel.DataAnnotations;

namespace AuthTrainning.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }
}
