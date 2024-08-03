using DummyServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace MumsDiceGame.NewFoHublder
{
    /// <summary>
    /// SignIn hub.
    /// TODO: implement with a REST signInService - look at network failures etc.
    /// </summary>
    public class SignInHub : Hub
    {
        public static readonly string SignInResponseId = "SignInResponse";
        public static readonly string SignOutResponseId = "SignOutResponse";

        private readonly ISignInService signInService;
        private readonly ILogger<SignInHub> logger;

        public SignInHub(ILogger<SignInHub> logger, ISignInService service) 
        { 
            this.logger = logger;
            signInService = service; 
        }

        public IHttpConnectionFeature? ConnectionFeature => Context?.Features.Get<IHttpConnectionFeature>();

        public async Task SignIn(string userJson)
        {
            logger.LogDebug(@"{SignIn}({userJson}) started", nameof(SignIn), userJson);

            var user = GameUserHelpers.GameUserFromJson(userJson ?? string.Empty);

            SimpleResponse response = new();
            if (user == null)
            {
                logger.LogError(@"{SignIn}({userJson}) invalid user", nameof(SignIn), userJson);
                response.Error = $"{nameof(SignIn)}({userJson}) user is invalid.";
            }
            else
            {
                var res = await signInService.SignIn(user);
                if (res.IsSuccess)
                {
                    System.Diagnostics.Debug.Assert(res.Item, "The false case should result in an error string - not the setting of the item which is a dummy value");
                    logger.LogDebug(@"{SignIn} signed in {user}", nameof(SignIn), user);
                }
                else
                {
                    logger.LogError(@"{SignIn}({user}) failed with error: {error}", nameof(SignIn), user, res.Error);
                    response.Error = res.Error;
                }
            }

            var resJson = JsonConvert.SerializeObject(response);
            logger.LogDebug($"{SignIn}({userJson}) sending response: {SignInResponseId}, {userJson}, {resJson}", 
                nameof(SignIn), userJson, SignInResponseId, userJson, resJson);
            await Clients.Caller.SendAsync(SignInResponseId, userJson, resJson);
        }

        public async Task SignOut(string userJson)
        {
            logger.LogDebug(@"{SignOut}({userJson}) started", nameof(SignOut), userJson);

            var user = GameUserHelpers.GameUserFromJson(userJson ?? string.Empty);

            SimpleResponse response = new();
            if (user == null)
            {
                logger.LogError(@"{SignOut}({userJson}) invalid user", nameof(SignOut), userJson);
                response.Error = $"{nameof(SignOut)}({userJson}) user is invalid.";
            }
            else
            {
                var res = await signInService.SignOut(user);
                if (res.IsSuccess)
                {
                    System.Diagnostics.Debug.Assert(res.Item, "The false case should result in an error string - not the setting of the item which is a dummy value");
                    logger.LogDebug(@"{SignOut} signed out {user}", nameof(SignOut), user);
                }
                else
                {
                    logger.LogError(@"{SignOut}({user}) failed with error: {error}", nameof(SignOut), user, res.Error);
                    response.Error = res.Error;
                }
            }

            var resJson = JsonConvert.SerializeObject(response);
            logger.LogDebug($"{SignOut}({userJson}) sending response: {SignOutResponseId}, {userJson}, {resJson}",
                nameof(SignOut), userJson, SignOutResponseId, userJson, resJson);
            await Clients.Caller.SendAsync(SignOutResponseId, userJson, resJson);
        }
    }
}
