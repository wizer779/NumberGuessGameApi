using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public int Age { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public Player()
        {
        }
    }
}
