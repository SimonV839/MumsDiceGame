using System.Net;

namespace DummyServices
{
    /// <summary>
    /// As user identifier
    /// </summary>
    /// <param name="Username">the name of the user</param>
    /// <param name="UserAddress">the ip address of the user</param>
    public record GameUser(string Username, IPAddress UserAddress);
}
