namespace MartinCostello.Api.Models
{
    /// <summary>
    /// A class representing custom error details. This class cannot be inherited.
    /// </summary>
    public sealed class ErrorDetail
    {
        /// <summary>
        /// Gets or sets the request Id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
