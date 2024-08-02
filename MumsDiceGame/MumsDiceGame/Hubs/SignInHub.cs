using DummyServices;
using Microsoft.AspNetCore.SignalR;

namespace MumsDiceGame.NewFoHublder
{
    /// <summary>
    /// SignIn hub.
    /// TODO: implement with a REST signInService - look at network failures etc.
    /// </summary>
    public class SignInHub : Hub
    {
        private readonly ILogger<SignInHub> logger;
        private readonly ISignInService signInService;

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public SignInHub(ILogger<SignInHub> logger, ISignInService service) 
        { 
            this.logger = logger;
            signInService = service; 
        }

        public async Task SignIn(GameUser user)
        {
            logger.LogDebug($"{SignIn} started for {user}", nameof(SignIn), user);

            SimpleResponse response = new();

            var res = await signInService.SignIn(user);
            if (!res.IsSuccess || !string.IsNullOrEmpty(res.Error))
            {
                logger.LogError(@"{SignIn}({user}) failed with error: {error}", nameof(SignIn), user, res.Error);
                response.Error = res.Error;
            }
            else if (res.Item == false)
            {
                logger.LogError(@"{SignIn}({user}) failed with unspecified error", nameof(SignIn), user);
                response.Error = $"{nameof(SignIn)} failed but did not specify why";
            }
            else
            {
                logger.LogDebug($"{SignIn} signed in {user}", nameof(SignIn), user);
            }

            await Clients.Caller.SendAsync("ReceiveMessage", user, response);
        }
    }
}
