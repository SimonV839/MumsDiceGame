using DummyServices;
using HubHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace MumsDiceGame.Client.HubClients
{
    public class SignInHubClient : IAsyncDisposable
    {
        #region Implementation
        private HubConnection? hubConnection;
        private HubExecutor? hubExecutor;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<SignInHubClient> logger;
        private readonly NavigationManager navigationManager;
        #endregion Implementation

        #region Public Interface
        public SignInHubClient(ILoggerFactory loggerFactory, NavigationManager navigationManager) 
        { 
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<SignInHubClient>();
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
            hubExecutor = new(loggerFactory.CreateLogger<HubExecutor>() , hubConnection);
        }

        public Task<ServiceResponse<ICollection<GameUser>>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> IsSignedIn(GameUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the sign in response notification.
        /// </summary>
        /// <param name="userJson">the first argument (representing the user in json form)</param>
        /// <param name="resJson">the second argument (representing the result in json form)</param>
        /// <returns>a service response indicting success of failure. The bool value has not significance other than being true on success else null.</returns>
        private ServiceResponse<bool> HandleSignInResponse(string userJson, string resJson)
        {
            var user = GameUserHelpers.GameUserFromJson(userJson);
            var res = JsonConvert.DeserializeObject<SimpleResponse>(resJson);

            var signInResponse = new ServiceResponse<bool>();
            if (user == null || res == null)
                { signInResponse.Error = $"{nameof(SignIn)}({user}) invalid data received."; }
            else if (!string.IsNullOrEmpty(res.Error))
                { signInResponse.Error = res.Error; }
            else
                { signInResponse.Item = res.IsSuccess; }

            return signInResponse;
        }

        /// <summary>
        /// Asynchronously sign the user in.
        /// </summary>
        /// <param name="user">the user to be signed in</param>
        /// <returns>a future service response indicating success or failure</returns>
        public async Task<ServiceResponse<bool>> SignIn(GameUser user)
        {
            logger?.LogDebug($"{SignIn}({user}) started", nameof(SignIn), user);

            if (hubConnection == null || hubExecutor == null)
            {
                logger?.LogDebug(@"{SignIn} rejected as {StartAsync} has not been called", nameof(SignIn), nameof(StartAsync));
                return new ServiceResponse<bool> { Error = "StartAsync must be called before attempting any other call." };
            }

            var userJson = user.ToJson();
            var res = await hubExecutor.SendAndProcessResponse("SignIn", userJson, "SignInResponse", HandleSignInResponse);

            return res;
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
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync().ConfigureAwait(false);
                hubConnection = null;
            }
        }
        #endregion IAsyncDisposable
    }
}
