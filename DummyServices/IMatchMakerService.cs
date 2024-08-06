namespace SimonV839.DummyServices
{
    public interface IMatchMakerService
    {
        Task<ServiceResponse<int>> RequestAnyOpponent();

        Task<ServiceResponse<int>> RequestSpecificOpponent(string opponentName);

        Task<ServiceResponse<bool>> CancelRequest(int requestId);

        Task<ServiceResponse<DiceMatch>> GetCurrentMatch();

        event EventHandler<GenericEventArgs<DiceMatch>> MatchChange;
    }
}
