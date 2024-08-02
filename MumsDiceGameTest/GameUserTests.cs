using DummyServices;

namespace MumsDiceGameTest
{
    [TestClass]
    public class GameUserTests
    {
        [TestMethod]
        public void Equality()
        {
            var one = new GameUser("fred", new System.Net.IPAddress(12));
            var two = new GameUser("fred", new System.Net.IPAddress(12));
            Assert.IsTrue(one.Equals(two));
        }

        [TestMethod]
        public void Identity()
        {
            var one = new GameUser("fred", new System.Net.IPAddress(12));
            var two = new GameUser("fred", new System.Net.IPAddress(12));
            Assert.IsFalse(one == two);
        }

        [TestMethod]
        public void InEquality()
        {
            {
                var one = new GameUser("red", new System.Net.IPAddress(12));
                var two = new GameUser("fred", new System.Net.IPAddress(12));
                Assert.IsFalse(one.Equals(two));
            }

            {
                var one = new GameUser("fred", new System.Net.IPAddress(120));
                var two = new GameUser("fred", new System.Net.IPAddress(12));
                Assert.IsFalse(one.Equals(two));
            }
        }

        [TestMethod]
        public void ToJson()
        {
            GameUser user = new ("fred", new System.Net.IPAddress(12));
            var json = user.ToJson();
            Assert.AreEqual(json, "{\r\n  \"UserName\": \"fred\",\r\n  \"UserAddress\": \"12.0.0.0\"\r\n}");
        }

        [TestMethod]
        public void FromJson()
        {
            var user = GameUserHelpers.GameUserFromJson("{\r\n  \"UserName\": \"fred\",\r\n  \"UserAddress\": \"12.0.0.0\"\r\n}");
            Assert.AreEqual(user, new GameUser("fred", new System.Net.IPAddress(12)));
        }
    }
}
