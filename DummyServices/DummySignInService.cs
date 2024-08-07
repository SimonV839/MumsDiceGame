
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace SimonV839.DummyServices
{
    public class DummySignInService : ISignInService, IMatchMakerService
    {
        private ILogger<DummySignInService> logger;
        private static readonly int DummyServerDelay = 1000;

        private readonly List<GameUser> users = new ();
        private readonly Dictionary<int, GameRequest> gameRequests = new ();
        private readonly List<Game> games = new();

        private record GameRequest(GameUser gameUser, string? opponentName);
        private record Game(GameUser player1, GameUser player2);

        private int lastGameId = 0;

        #region ISignInService
        public DummySignInService(ILogger<DummySignInService> logger)
        {
            this.logger = logger;

            logger.LogDebug(@"{DummySignInService} constructed", nameof(DummySignInService));
        }

        public event EventHandler OnChange = null!;

        public async Task<ServiceResponse<ICollection<GameUser>>> GetUsers()
        {
            await Task.Delay(DummyServerDelay);

            return new ServiceResponse<ICollection<GameUser>>() { Item = GetAllUsers() };
        }

        public async Task<ServiceResponse<bool>> IsSignedIn(GameUser user)
        {
            await Task.Delay(DummyServerDelay);

            lock (users)
            {
                return new ServiceResponse<bool>() { Item = users.Contains(user) };
            }
        }

        public async Task<ServiceResponse<bool>> SignIn(GameUser user)
        {
            await Task.Delay(DummyServerDelay);

            var isNotPresent = true;
            lock (users)
            {
                if (users.Contains(user) || users.Any(val => val.UserName == user.UserName))
                {
                    isNotPresent = false;
                }
                else
                {
                    users.Add(user);
                }
            }

            var res = new ServiceResponse<bool>();
            if (isNotPresent) 
            { 
                res.Item = true;
                NotifyChange(); 
            }
            else
            {
                res.Item = false;
                res.Error = $"SignIn({user}) failed because that user is already signed in";
            }

            return res;
        }

        public async Task<ServiceResponse<bool>> SignOut(GameUser user)
        {
            await Task.Delay(DummyServerDelay);

            bool isRemoved;
            lock (users)
            {
                isRemoved = users.Remove(user);
            }
            if (isRemoved) { NotifyChange(); }
            return new ServiceResponse<bool>() { Item = isRemoved };
        }
        #endregion ISignInService

        #region IMatchMakerService
        // todo do with db service
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameUser"></param>
        /// <returns>the id of the new match, else 0 indicating a request is pending... or error </returns>
        public async Task<ServiceResponse<int>> RequestAnyOpponent(GameUser gameUser)
        {
            throw new Exception("");
            /*
            GameRequest? existingReq;
            Game? newGame;
            lock(users)
            {
                lock(gameRequests)
                {
                    // already waiting
                    if (gameRequests.Any(req => 
                        req.Value.gameUser.UserName.Equals(gameUser.UserName)
                        ))
                    {
                        return new ServiceResponse<int>() { Error = $"{gameUser.UserName} has already registered for a game." };
                    }

                    // being waited apon
                    var req = gameRequests.FirstOrDefault(req =>
                        !req.Value.gameUser.UserName.Equals(gameUser.UserName));
                    if (req.Value == null)
                    {
                        var id = ++lastGameId;
                        gameRequests.Add(id, new GameRequest(gameUser, string.Empty));

                        return new ServiceResponse<int>() { Item = id };
                    }

                    if (games.Any(game => 
                        game.player1.Equals(req.Value.opponentName) 
                        || game.player1.Equals(gameUser.UserName)
                        || game.player2.Equals(req.Value.opponentName)
                        || game.player2.Equals(gameUser.UserName)))
                    {
                        return new ServiceResponse<int>() { Error = $"{gameUser.UserName} has already registered for a game." };
                    }

                    if ()
                    {

                    }
                    existingReq = req.Value;
                    gameRequests.Remove(req.Key);
                    newGame = new Game(existingReq.gameUser, gameUser);
                    games.Add(newGame);
                }
            }

            // inform opponent

            // inform requestor
            */
        }

        public Task<ServiceResponse<int>> RequestSpecificOpponent(string opponentName)
        {
            throw new Exception("todo");
        }

        public Task<ServiceResponse<bool>> CancelRequest(int requestId)
        {
            throw new Exception("todo");
        }

        public Task<ServiceResponse<DiceMatch>> GetCurrentMatch()
        {
            throw new Exception("todo");
        }

        public event EventHandler<GenericEventArgs<DiceMatch>> MatchChange = null!;
        #endregion IMatchMakerService

        #region Implementation
        private void NotifyMatchChange(DiceMatch match)
        {
            MatchChange?.Invoke(this, new GenericEventArgs<DiceMatch>(match));
        }
        private void NotifyChange()
        {
            //OnChange?.BeginInvoke(this, new EventArgs(), null, null); 
            OnChange?.Invoke(this, new EventArgs());
        }

        private ICollection<GameUser> GetAllUsers()
        {
            lock (users)
            {
                return users;
            }
        }
        // todo - implement with db - for now concentrating on serivce
        public GameUser? GetUserByName(string userName)
        {
            lock (users)
            {
                return users.First(u => u.UserName.Equals(userName));
            }
        }
        #endregion Implementation
    }
}
