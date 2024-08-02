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
        public static readonly string SignInId = "SignIn";
        public static readonly string SignInResponseId = "SignInResponse";

        private readonly ISignInService signInService;

        private ILogger<SignInHub> logger;

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
                response.Error = $"{nameof(SignIn)}({{userJson}}) user is invalid.";
            }
            else
            {
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
                    logger.LogDebug(@"{SignIn} signed in {user}", nameof(SignIn), user);
                }
            }

            var resJson = JsonConvert.SerializeObject(response);
            logger.LogDebug($"{SignIn}({userJson}) sending response: {SignInResponseId}, {userJson}, {resJson}", 
                nameof(SignIn), userJson, SignInResponseId, userJson, resJson);
            await Clients.Caller.SendAsync(SignInResponseId, userJson, resJson);
        }
    }
}
