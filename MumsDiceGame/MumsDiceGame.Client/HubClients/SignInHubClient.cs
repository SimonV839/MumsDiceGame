using DummyServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace MumsDiceGame.Client.HubClients
{
    public class SignInHubClient : IAsyncDisposable
    {
        #region Implementation
        private HubConnection? hubConnection;
        private readonly NavigationManager navigationManager;

        private IDisposable? signInSubscription;
        private readonly ManualResetEvent signInResponseEvent = new(false);
        private SimpleResponse? signInResponse;

        private ILogger<SignInHubClient> logger;
        #endregion Implementation

        #region Public Interface
        public SignInHubClient(ILogger<SignInHubClient> logger, NavigationManager navigationManager) 
        { 
            this.logger = logger;
            this.navigationManager = navigationManager;
        }

        public HubConnectionState? HubConnectionState => hubConnection?.State;

        public async Task StartAsync()
        {
            if (hubConnection != null)
            {
                System.Diagnostics.Debug.Assert(false, "Start should only be called once");
                return;
            }

            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/signinhub"))
                .Build();

            await hubConnection.StartAsync();
        }

        public Task<ServiceResponse<ICollection<GameUser>>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> IsSignedIn(GameUser user)
        {
            throw new NotImplementedException();
        }

        private void HandleSignInResponse(string userJson, string resJson)
        {
            var user = GameUserHelpers.GameUserFromJson(userJson);
            var res = JsonConvert.DeserializeObject<SimpleResponse>(resJson);

            System.Diagnostics.Debug.Assert(signInResponse == null, "The signInResponse should be null here. Check threading.");

            signInResponse = new SimpleResponse();
            if (user == null || res == null)
            {
                signInResponse.Error = $"{nameof(SignIn)}({user}) invalid data received.";
            }
            else
            {
                signInResponse.Error = res.Error;
            }

            logger?.LogDebug($"{SignIn} response from hub: {user}, {res}. triggering {signInResponseEvent}",
                nameof(SignIn), user, res, nameof(signInResponseEvent));
            signInResponseEvent.Set();
        }

        private static async Task AwaitEvent(EventWaitHandle ev)
        {
            var task = new Task(() => ev.WaitOne() );
            task.Start();
            await task;
        }

        public async Task<SimpleResponse> SignIn(GameUser user)
        {
            logger?.LogDebug($"{SignIn}({user}) started", nameof(SignIn), user);

            if (hubConnection == null)
            {
                logger?.LogDebug(@"{SignIn} rejected as {StartAsync} has not been called", nameof(SignIn), nameof(StartAsync));
                return new SimpleResponse { Error = "StartAsync must be called before attempting any other call." };
            }

            if (signInSubscription != null)
            {
                logger?.LogDebug(@"{SignIn} rejected as previous attempt has not completed", nameof(SignIn));
                return new SimpleResponse { Error = "SignIn rejected as previous attempt has not completed" };
            }

            signInResponseEvent.Reset();
            signInResponse = null;
            signInSubscription = hubConnection.On<string, string>("SignInResponse", HandleSignInResponse);

            var userJson = user.ToJson();
            await hubConnection.SendAsync("SignIn", userJson);

            logger?.LogDebug($"{nameof(SignIn)}({user}) waiting for hub response");
            await AwaitEvent(signInResponseEvent);

            signInSubscription.Dispose();
            signInSubscription = null;

            System.Diagnostics.Debug.Assert(signInResponse != null, "at this point the response from the server must have been received");
            return signInResponse;
        }

        public Task<ServiceResponse<bool>> SignOut(GameUser user)
        {
            throw new NotImplementedException();
        }
        #endregion Public Interface

        #region IAsyncDisposable
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (signInSubscription != null)
            {
                signInSubscription.Dispose();
                signInSubscription = null;
            }
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync().ConfigureAwait(false);
                hubConnection = null;
            }
        }
        #endregion IAsyncDisposable
    }
}
