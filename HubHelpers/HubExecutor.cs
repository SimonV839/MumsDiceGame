using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using DummyServices;
using Microsoft.AspNetCore.SignalR.Protocol;
using static HubHelpers.HubExecutor;

namespace HubHelpers
{
    /// <summary>
    /// Class which executes hub methods and receives and actions the response.
    /// </summary>
    public class HubExecutor
    {
        public delegate ServiceResponse<T> ResponseHandler<T>(string responseId, string responseMessage) where T: new();

        private readonly ILogger<HubExecutor>? logger;
        private readonly HubConnection hubConnection;

        /// <summary>
        /// Class allowing the user supplied function to not have to know about
        /// the completion event.
        /// </summary>
        /// <typeparam name="T">the type of the result</typeparam>
        private class HubResponder<T> where T: new()
        {
            /// <summary>
            /// Method that will process a received message and return a result for that processing.
            /// </summary>
            private readonly ResponseHandler<T> userResponseHandler;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="userResponseHandler">function that will process the response and supply the result of that processing</param>
            public HubResponder(ResponseHandler<T> userResponseHandler)
            {
                this.userResponseHandler = userResponseHandler;
            }

            /// <summary>
            /// The result of handling the response
            /// </summary>
            public ServiceResponse<T> Result { get; private set; } = new ServiceResponse<T>() { Error = "Not set" };

            /// <summary>
            /// The event used to signal completion of handling the response
            /// </summary>
            public ManualResetEvent CompletionEvent { get; private set; } = new(false);

            /// <summary>
            /// Method called by the hub
            /// </summary>
            /// <param name="command">the command field of the response</param>
            /// <param name="data">the data associated with the response</param>
            public void InternalResponseHandler(string command, string data)
            {
                Result = userResponseHandler(command, data);
                CompletionEvent.Set();
            }
        }

        public HubExecutor(ILogger<HubExecutor>? logger, HubConnection hubConnection) 
        {
            this.logger = logger;
            this.hubConnection = hubConnection;
        }

        private static async Task AwaitEvent(EventWaitHandle ev)
        {
            var task = new Task(() => ev.WaitOne());
            task.Start();
            await task;
        }

        public async Task<ServiceResponse<T>> SendAndProcessResponse<T>(
            string sendId, string dataJson, string receiveId, ResponseHandler<T> responseHandler)
            where T : new()
        {
            if (hubConnection == null)
            {
                return new ServiceResponse<T> { Error = "The hub must be started before attempting to send a message." };
            }

            var responder = new HubResponder<T>(responseHandler);

            var subscription = hubConnection.On<string, string>(sendId, responder.InternalResponseHandler);

            await hubConnection.SendAsync(sendId, dataJson);

            await AwaitEvent(responder.CompletionEvent);

            subscription.Dispose();

            return responder.Result;
        }
    }
}
