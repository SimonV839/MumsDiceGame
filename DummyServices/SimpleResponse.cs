namespace SimonV839.DummyServices
{
    /// <summary>
    /// Service Response
    /// Simon: TODO 555 Because I don't know better yet
    /// </summary>
    public class SimpleResponse
    {
        /// <summary>
        /// Whether this indicates success of failure
        /// </summary>
        public bool IsSuccess
        {
            get => Error == null;
        }

        /// <summary>
        /// The text associated with a failure.
        /// </summary>
        public string? Error { get; set; }
    }
}
