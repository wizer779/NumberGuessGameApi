using NumberGuessGameApi.DataTransferObjects;

namespace NumberGuessGameApi.Services
{
    public interface IGameService
    {
        Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request);

        Task<StartGameResponse> StartGameAsync(StartGameRequest request);

        Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request);
    }
}
