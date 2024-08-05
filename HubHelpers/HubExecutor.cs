using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using SimonV839.DummyServices;

namespace HubHelpers
{
    /// <summary>
    /// Class which asynchronously executes a hub method and awaits a notification (taken as the response to the request).
    /// The user supplies a delegate which will handle the notification.
    /// Currently:
    /// 1 argument hub method and 2 argument response notification is supported.
    /// </summary>
    public class HubExecutor
    {
        /// <summary>
        /// Delegate for a 2 parameter notification handler
        /// </summary>
        /// <typeparam name="T">the type of the result that will be returned in the ServiceResponse</typeparam>
        /// <param name="arg1">the first argument in the notification</param>
        /// <param name="arg2">the second argument in the notification</param>
        /// <returns>A ServiceResponse which contains the computed value or and error.</returns>
        public delegate ServiceResponse<T> Notification2ArgHandler<T>(string arg1, string arg2) where T: new();

        /// <summary>
        /// Initialises a new instance of the HubExecutor.
        /// </summary>
        /// <param name="logger">the logger to be used</param>
        /// <param name="hubConnection">the hub connection on which methods will be called a from which notifications will be received</param>
        public HubExecutor(ILogger<HubExecutor>? logger, HubConnection hubConnection) 
        {
            this.logger = logger;
            this.hubConnection = hubConnection;
        }

        /// <summary>
        /// Asynchronously call a method and await a notification response.
        /// </summary>
        /// <typeparam name="T">Type of the data extracted from the response</typeparam>
        /// <param name="hubMethodName">the name of the hub method name to be called</param>
        /// <param name="hubMethodArg1">argument for the hub method</param>
        /// <param name="notificationMethodName">the notification method name (id of the response)</param>
        /// <param name="notificationHandler">a delegate that will be called with the response notification data</param>
        /// <returns>a service response with the returned data or an error</returns>
        public async Task<ServiceResponse<T>> SendAndProcessResponse<T>(
            string hubMethodName, string hubMethodArg1, 
            string notificationMethodName, Notification2ArgHandler<T> notificationHandler)
            where T : new()
        {
            if (hubConnection == null)
            {
                return new ServiceResponse<T> { Error = "The hub must be started before attempting to send a message." };
            }

            var responder = new HubNotificationResponder<T>(notificationHandler);

            using (var subscription = hubConnection.On<string, string>(notificationMethodName, responder.InternalResponseHandler))
            {
                await hubConnection.SendAsync(hubMethodName, hubMethodArg1);

                await AwaitEvent(responder.CompletionEvent);
            }

            return responder.Result;
        }

        #region Implementation
        private readonly ILogger<HubExecutor>? logger;
        private readonly HubConnection hubConnection;

        /// <summary>
        /// Class encapsulating a notification handler, its result and the event signalling processing of the notification.
        /// the completion event.
        /// </summary>
        /// <typeparam name="T">the type of the result of processing</typeparam>
        private class HubNotificationResponder<T> where T : new()
        {
            /// <summary>
            /// Initialises a new instance of the HubNotificationResponder class.
            /// </summary>
            /// <param name="notificationHandler">delegate that will process the notification (response) data and supply the result of that processing.</param>
            public HubNotificationResponder(Notification2ArgHandler<T> notificationHandler)
            {
                this.notificationHandler = notificationHandler;
            }

            /// <summary>
            /// The result of handling the notification (response).
            /// </summary>
            public ServiceResponse<T> Result { get; private set; } = new ServiceResponse<T>() { Error = "Not set" };

            /// <summary>
            /// The event used to signal completion of handling the response.
            /// This will be set to indicate completion.
            /// </summary>
            public ManualResetEvent CompletionEvent { get; private set; } = new(false);

            /// <summary>
            /// Method called when the hub notification is received.
            /// </summary>
            /// <param name="notificationArg1">the notificationArg1 field of the response</param>
            /// <param name="notificationArg2">the notificationArg2 associated with the response</param>
            public void InternalResponseHandler(string notificationArg1, string notificationArg2)
            {
                Result = notificationHandler(notificationArg1, notificationArg2);
                CompletionEvent.Set();
            }

            /// <summary>
            /// Delegate that will process a notification data return a result for that processing.
            /// </summary>
            private readonly Notification2ArgHandler<T> notificationHandler;
        }

        private static async Task AwaitEvent(EventWaitHandle ewh)
        {
            var task = new Task(() => ewh.WaitOne());
            task.Start();
            await task;
        }
        #endregion Implementation
    }
}
