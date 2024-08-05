using System.Diagnostics.Contracts;

namespace SimonV839.DummyServices
{
    /// <summary>
    /// Service Response
    /// Simon: TODO 555 Because I don't know better yet.
    /// </summary>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Whether this indicates success of failure
        /// </summary>
        public bool IsSuccess
        {
            get => simpleResponse.IsSuccess;
        }

        /// <summary>
        /// The text associated with a failure.
        /// 
        /// </summary>
        public string? Error 
        { 
            get => simpleResponse.Error;
            set
            {
                Contract.Requires(IsValid, "The combination of allowable values is incorrect.");
                Contract.Ensures(IsValid, "The combination of allowable values is incorrect.");
                simpleResponse.Error = value;
            }
        }

        /// <summary>
        /// The time associated with success.
        /// </summary>
        public T? Item 
        {
            get
            {
                Contract.Requires(IsValid, "The combination of allowable values is incorrect.");
                return item;
            }
            set
            {
                Contract.Requires(IsValid, "The combination of allowable values is incorrect.");
                Contract.Ensures(IsValid, "The combination of allowable values is incorrect.");

                item = value;
                if (value == null)
                {
                    if (Error == null)
                    {
                        simpleResponse.Error = string.Empty;
                    }
                }
                else
                {
                    simpleResponse.Error = null;
                }
            }
        }

        private bool IsValid
        {
            get
            {
                return
                    (simpleResponse.IsSuccess && !EqualityComparer<T>.Default.Equals(item, default)) ||
                    (!simpleResponse.IsSuccess && EqualityComparer<T>.Default.Equals(item, default));
            }
        }

        private SimpleResponse simpleResponse = new SimpleResponse();
        private T? item;
    }
}
