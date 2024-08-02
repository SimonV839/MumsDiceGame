using DummyServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace MumsDiceGame.Client.NewFolder
{
    public class SignInHubClient
    {
        private readonly ILogger<SignInHubClient> logger;
        private readonly HubConnection hubConnection;
        private readonly NavigationManager navigationManager;

        private IDisposable? signInSubscription;
        private AutoResetEvent signInResponseEvent = new AutoResetEvent(false);
        private SimpleResponse? signInResponse;


        public SignInHubClient(ILogger<SignInHubClient> logger, NavigationManager navigationManager) 
        { 
            this.logger = logger;
            this.navigationManager = navigationManager;

            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/chathub"))
                .Build();
        }

        public Task<ServiceResponse<ICollection<GameUser>>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> IsSignedIn(GameUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<SimpleResponse> SignIn(GameUser user)
        {
            if (signInSubscription != null)
            {
                logger.LogDebug(@"{SignIn} rejected as previous attempt has not completed", nameof(SignIn));
                return new SimpleResponse { Error = "SignIn rejected as previous attempt has not completed" };
            }

            signInResponseEvent.Reset();
            signInSubscription = hubConnection.On<GameUser, SimpleResponse>("SignInResponse",
                (user, res) => 
                {
                    System.Diagnostics.Debug.Assert(signInResponse == null, "The signInResponse should be null here. Check threading.");

                    signInResponse = new SimpleResponse() { Error = res.Error };
                    logger.LogDebug($"{nameof(SignIn)} response from hub: {user}, {res}");
                    signInResponseEvent.Set();
                });

            logger.LogDebug($"{nameof(SignIn)}({user}) waiting for hub response");
            await new Task(() => { signInResponseEvent.WaitOne(); });

            signInSubscription.Dispose();
            signInSubscription = null;

            System.Diagnostics.Debug.Assert(signInResponse != null, "at this point the response from the server must have been received");
            return signInResponse;
        }

        public Task<ServiceResponse<bool>> SignOut(GameUser user)
        {
            throw new NotImplementedException();
        }
    }
}
