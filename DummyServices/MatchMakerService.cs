
using Microsoft.Extensions.Logging;

namespace SimonV839.DummyServices
{
    public class MatchMakerService : IMatchMakerService
    {
        private readonly ILogger<MatchMakerService> logger;
        private readonly DummySignInService signInService;
        //  when locking, lock matches then users to avoid deadlock - todo replace with db
        private readonly List<DiceMatch> matches = new List<DiceMatch>();    //  todo - concentrating on services for now. Later move to a db
        private readonly List<GameUser> users = new List<GameUser>();

        public MatchMakerService(ILogger<MatchMakerService> logger, DummySignInService signInService)
        {
            this.logger = logger;
            this.signInService = signInService;

            this.signInService.OnChange += SignInService_OnChange;
        }

        #region IMatchMakerService
        private void SignInService_OnChange(object? sender, EventArgs e)
        {
            var addedUsers = new List<GameUser>();
            var removedUsers = new List<GameUser>();

            lock (matches)
            {
                lock (users)
                {
                    var newUsers = signInService.GetAllUsers();
                    foreach (var item in newUsers)
                    {
                        //if (users)
                    }
                }
            }
        }

        public event EventHandler<GenericEventArgs<DiceMatch>>? MatchChange;

        public Task<ServiceResponse<bool>> CancelRequest(int requestId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<DiceMatch>> GetCurrentMatch()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> RequestAnyOpponent()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> RequestSpecificOpponent(string opponentName)
        {
            throw new NotImplementedException();
        }
        #endregion IMatchMakerService

    }
}
