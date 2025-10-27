using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    public class RegisterPlayerRequest
    {
        [Required]
        [StringLength(50)]
        [MinLength(1)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [MinLength(1)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }
    }
}
