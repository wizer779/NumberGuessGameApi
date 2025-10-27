using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.Models
{
    public class Attempt
    {
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }

        public virtual Game Game { get; set; } = null!;

        [Required]
        public string AttemptedNumber { get; set; } = string.Empty;

        [Required]
        public string ResultMessage { get; set; } = string.Empty;

        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;

        public Attempt()
        {
        }
    }
}
