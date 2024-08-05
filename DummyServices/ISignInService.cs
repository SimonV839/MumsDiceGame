
namespace SimonV839.DummyServices
{
    /// <summary>
    /// Sign in service.
    /// Allows users to:
    ///  - attempt to sign in with  username and IPAddress which must be unique at the point of signing in,
    ///  - attemt to sign out with username and IPAddress which must the sign in request.
    ///  
    /// This is intended for only a few users.
    /// </summary>
    public interface ISignInService
    {
        /// <summary>
        /// Attempt to sign in.
        /// </summary>
        /// <param name="user">the user to be signed in</param>
        /// <returns>the response value have IsSuccess of success. Failure may be due to the user not being unique</returns>
        Task<ServiceResponse<bool>> SignIn(GameUser user);

        /// <summary>
        /// Attempt to sign out.
        /// </summary>
        /// <param name="user">the user to be signed out</param>
        /// <returns>the response value have IsSuccess on success. Failure may be due to the user not being signed in.</returns>
        Task<ServiceResponse<bool>> SignOut(GameUser user);

        /// <summary>
        /// Determines whether the user is signed in.
        /// </summary>
        /// <param name="user">the user to be checked</param>
        /// <returns>the response value will be true if signed in and no error reported.</returns>
        Task<ServiceResponse<bool>> IsSignedIn(GameUser user);

        /// <summary>
        /// Gets the full list of users
        /// </summary>
        /// <returns>the response value will be the list of users at the current time.</returns>
        Task<ServiceResponse<ICollection<GameUser>>> GetUsers();

        /// <summary>
        /// Event that fires when a user signs in or signs out.
        /// </summary>
        event EventHandler OnChange;
   }
}
