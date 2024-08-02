
using Microsoft.Extensions.Logging;

namespace DummyServices
{
    public class DummySignInService : ISignInService
    {
        private ILogger<DummySignInService> logger;
        private static readonly int DummyServerDelay = 500;

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
                if (users.Contains(user) || users.Any(val => val.Username == user.Username))
                {
                    isNotPresent = false;
                }
                else
                {
                    users.Add(user);
                }
            }
            if (isNotPresent) { NotifyChange(); }
            return new ServiceResponse<bool>() { Item = isNotPresent };
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
