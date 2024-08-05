using SimonV839.DummyServices;
using HubHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace SimonV839.MumsDiceGame.Client.HubClients
{
    public class SignInHubClient(ILoggerFactory loggerFactory, NavigationManager navigationManager) : IAsyncDisposable
    {
        #region Implementation
        private readonly ILoggerFactory loggerFactory = loggerFactory;
        private readonly ILogger<SignInHubClient> logger = loggerFactory.CreateLogger<SignInHubClient>();
        private readonly NavigationManager navigationManager = navigationManager;
        private HubConnection? hubConnection;
        private HubExecutor? hubExecutor;

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
        /// Handles the sign out response notification.
        /// </summary>
        /// <param name="userJson">the first argument (representing the user in json form)</param>
        /// <param name="resJson">the second argument (representing the result in json form)</param>
        /// <returns>a service response indicting success of failure. The bool value has not significance other than being true on success else null.</returns>
        private ServiceResponse<bool> HandleSignOutResponse(string userJson, string resJson)
        {
            var user = GameUserHelpers.GameUserFromJson(userJson);
            var res = JsonConvert.DeserializeObject<SimpleResponse>(resJson);

            var signOutResponse = new ServiceResponse<bool>();
            if (user == null || res == null)
            { signOutResponse.Error = $"{nameof(SignOut)}({user}) invalid data received."; }
            else if (!string.IsNullOrEmpty(res.Error))
            { signOutResponse.Error = res.Error; }
            else
            { signOutResponse.Item = res.IsSuccess; }

            return signOutResponse;
        }

        #endregion Implementation

        #region Public Interface

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

        public async Task<ServiceResponse<bool>> SignOut(GameUser user)
        {
            logger?.LogDebug($"{SignOut}({user}) started", nameof(SignOut), user);

            if (hubConnection == null || hubExecutor == null)
            {
                logger?.LogDebug(@"{SignOut} rejected as {StartAsync} has not been called", nameof(SignOut), nameof(StartAsync));
                return new ServiceResponse<bool> { Error = "StartAsync must be called before attempting any other call." };
            }

            var userJson = user.ToJson();
            var res = await hubExecutor.SendAndProcessResponse("SignOut", userJson, "SignOutResponse", HandleSignOutResponse);

            return res;
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
