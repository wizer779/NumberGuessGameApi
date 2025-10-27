namespace NumberGuessGameApi.DataTransferObjects
{
    public class GuessNumberResponse
    {
        public int GameId { get; set; }

        public string AttemptedNumber { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
