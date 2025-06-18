namespace MartinCostello.Api
{
    /// <summary>
    /// A class containing error codes. This class cannot be inherited.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// The entity already exists.
        /// </summary>
        public const string Exists = "Exists";

        /// <summary>
        /// An internal error occurred.
        /// </summary>
        public const string InternalError = "InternalError";

        /// <summary>
        /// An invalid parameter value was specified.
        /// </summary>
        public const string InvalidParameter = "InvalidParameter";

        /// <summary>
        /// A required parameter was not specified.
        /// </summary>
        public const string MissingParameter = "MissingParameter";

        /// <summary>
        /// No request body was specified.
        /// </summary>
        public const string NoRequestBody = "NoRequestBody";

        /// <summary>
        /// The entity was not found.
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// The resource is not implemented.
        /// </summary>
        public const string NotImplemented = "NotImplemented";

        /// <summary>
        /// The request timed-out.
        /// </summary>
        public const string Timeout = "Timeout";
    }
}
