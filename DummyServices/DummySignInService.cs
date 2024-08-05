
using Microsoft.Extensions.Logging;

namespace SimonV839.DummyServices
{
    public class DummySignInService : ISignInService
    {
        private ILogger<DummySignInService> logger;
        private static readonly int DummyServerDelay = 1000;

        private List<GameUser> users = new List<GameUser>();

        private void NotifyChange() 
        {
            //OnChange?.BeginInvoke(this, new EventArgs(), null, null); 
            OnChange?.Invoke(this, new EventArgs());
        }

        public DummySignInService(ILogger<DummySignInService> logger)
        {
            this.logger = logger;

            logger.LogDebug(@"{DummySignInService} constructed", nameof(DummySignInService));
        }

        public event EventHandler OnChange = null!;

        public async Task<ServiceResponse<ICollection<GameUser>>> GetUsers()
        {
            await Task.Delay(DummyServerDelay);

            lock(users)
            {
                return new ServiceResponse<ICollection<GameUser>>() { Item = users.ToList() };
            }
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
    }
}
