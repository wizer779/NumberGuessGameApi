using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    public class GuessNumberRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int GameId { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression("^[0-9]*$")]
        public string AttemptedNumber { get; set; } = string.Empty;
    }
}
