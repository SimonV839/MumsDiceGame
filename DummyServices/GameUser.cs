using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DummyServices
{
    /// <summary>
    /// As user identifier
    /// </summary>
    /// <param name="Username">the name of the user</param>
    /// <param name="UserAddress">the ip address of the user</param>
    public class GameUser : IEquatable<GameUser>
    {
        public GameUser(string username, IPAddress userAddress)
        {
            UserName = username;
            UserAddress = userAddress;
        }

        public string UserName { get; set; }
        public IPAddress UserAddress { get; set; }

        public override string ToString()
        {
            return $"UserName: '{UserName}', UserAddress: '{UserAddress}'";
        }

        #region IEquatable
        public override bool Equals(object? obj)
        {
            var other = obj as GameUser;
            if (other == null) return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = UserName.GetHashCode();
            hash ^= UserAddress.GetHashCode();

            return hash;
        }

        public bool Equals(GameUser? other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // You can also use a specific StringComparer instead of EqualityComparer<string>
            // Check out the specific implementations (StringComparer.CurrentCulture, e.a.).
            if (string.Compare(UserName, other.UserName) != 0) { return false; }

            if (!UserAddress.Equals(other.UserAddress)) { return false; }

            return true;
        }
        #endregion IEquatable
    }

    class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader?.Value);
        }
    }
    class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            JObject jo = new JObject();

            if (value == null) { jo.Add(null); }
            else
            {
                IPEndPoint ep = (IPEndPoint)value;

                jo.Add("Address", JToken.FromObject(ep.Address, serializer));
                jo.Add("Port", ep.Port);
                jo.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            return new IPEndPoint(address, port);
        }
    }
    public static class GameUserHelpers
    {
        public static GameUser? GameUserFromJson(string jsonString)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new IPEndPointConverter());
            settings.Formatting = Formatting.Indented;

            return JsonConvert.DeserializeObject<GameUser>(jsonString, settings);
        }

        public static string ToJson(this GameUser user)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new IPEndPointConverter());
            settings.Formatting = Formatting.Indented;

            return JsonConvert.SerializeObject(user, settings);
        }
    }
}
