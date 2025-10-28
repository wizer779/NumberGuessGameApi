using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        public virtual Player Player { get; set; } = null!;

        [Required]
        public string SecretNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public GameStatus Status { get; set; } = GameStatus.Active;

        public virtual ICollection<Attempt> Attempts { get; set; }

        public Game()
        {
            Attempts = new List<Attempt>();
        }
    }
}
