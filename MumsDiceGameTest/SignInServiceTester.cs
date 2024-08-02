using DummyServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MumsDiceGameTest
{
    [TestClass]
    public class SignInServiceTester
    {
        [TestMethod]
        public void SignInSignOutSuccess()
        {
            DummySignInService signInService = new( new NullLoggerFactory().CreateLogger<DummySignInService>());

            {
                var val = signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "a unique name and address should succeed");
            }

            {
                var val = signInService.SignIn(new GameUser("jane", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "a unique name should succeed");
            }

            {
                var val = signInService.SignOut(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "a signed in user should be able to sign out once");
            }

            {
                var val = signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "a signed out user should be able to sign in again");
            }

            {
                var val = signInService.SignOut(new GameUser("jane", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "a signed in user should be able to sign out once");
            }
        }

        [TestMethod]
        public void SignInSignOutFailure()
        {
            DummySignInService signInService = new(new NullLoggerFactory().CreateLogger<DummySignInService>());

            signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12))).Wait();
            signInService.SignIn(new GameUser("jane", new System.Net.IPAddress(12))).Wait();

            {
                var val = signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "a duplicate name and address should fail");
            }

            {
                var val = signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(13)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "a duplicate name alone should fail");
            }

            {
                var val = signInService.SignOut(new GameUser("fred", new System.Net.IPAddress(13)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "a duplicate name alone should fail");
            }
        }

        [TestMethod]
        public void IsSignedIn()
        {
            DummySignInService signInService = new(new NullLoggerFactory().CreateLogger<DummySignInService>());

            {
                var val = signInService.IsSignedIn(new GameUser("fred", new System.Net.IPAddress(34)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "non signed in should not be reported signed in");
            }

            signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12))).Wait();
            signInService.SignIn(new GameUser("jane", new System.Net.IPAddress(12))).Wait();

            {
                var val = signInService.IsSignedIn(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && val.Result.Item, "previously signed in should still be signed in");
            }

            {
                signInService.SignOut(new GameUser("fred", new System.Net.IPAddress(12))).Wait();
                var val = signInService.IsSignedIn(new GameUser("fred", new System.Net.IPAddress(12)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "previously signed out should not be reported signed in");
            }

            {
                var val = signInService.IsSignedIn(new GameUser("fred", new System.Net.IPAddress(34)));
                Assert.IsTrue(val.Result.IsSuccess && !val.Result.Item, "non signed in should not be reported signed in");
            }
        }

        [TestMethod]
        public void GetSignedIn()
        {
            DummySignInService signInService = new(new NullLoggerFactory().CreateLogger<DummySignInService>());

            {
                var val = signInService.GetUsers();
                Assert.IsTrue(val.Result.IsSuccess &&
                    val.Result.Item != null &&
                    val.Result.Item.Count == 0,
                    "No users should be present");
            }

            signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12))).Wait();

            {
                var val = signInService.GetUsers();
                Assert.IsTrue(val.Result.IsSuccess &&
                    val.Result.Item != null &&
                    val.Result.Item.Count == 1,
                    "The added valud should be present");
                Assert.IsTrue(
                    val.Result.Item.First().Equals(new GameUser("fred", new System.Net.IPAddress(12))),
                    "The added valud should be present");
            }
        }

        private static int eventCount;
        private static void OnEvent(object? subject, EventArgs? args)
        {
            ++eventCount;
        }

        [TestMethod]
        public void ChangeEvent()
        {
            eventCount = 0;

            DummySignInService signInService = new(new NullLoggerFactory().CreateLogger<DummySignInService>());

            signInService.SignIn(new GameUser("fred", new System.Net.IPAddress(12))).Wait();
            Task.Delay(10);
            Assert.IsTrue(eventCount == 0, "No events should be called if not registered");

            signInService.OnChange += OnEvent;
            Task.Delay(10);
            Assert.IsTrue(eventCount == 0, "No events should be called just for registering");

            signInService.SignIn(new GameUser("jane", new System.Net.IPAddress(12))).Wait();
            Task.Delay(10);
            Assert.IsTrue(eventCount == 1, "signing in should trigger the event");

            signInService.SignOut(new GameUser("jane", new System.Net.IPAddress(12))).Wait();
            Task.Delay(10);
            Assert.IsTrue(eventCount == 2, "signing out should trigger the event");

            signInService.SignOut(new GameUser("jane", new System.Net.IPAddress(12))).Wait();
            Task.Delay(10);
            Assert.IsTrue(eventCount == 2, "failed sign out should not trigger the event");
        }
    }
}