namespace MartinCostello.Api.Models
{
    /// <summary>
    /// A class representing the response from the <c>/time</c> API resource. This class cannot be inherited.
    /// </summary>
    public sealed class TimeResponse
    {
        /// <summary>
        /// Gets or sets the current UTC date and time in RFC1123 format.
        /// </summary>
        public string Rfc1123 { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds since the UNIX epoch.
        /// </summary>
        public long Unix { get; set; }

        /// <summary>
        /// Gets or sets the current UTC date and time in universal sortable format.
        /// </summary>
        public string UniversalSortable { get; set; }

        /// <summary>
        /// Gets or sets the current UTC date and time in universal full format.
        /// </summary>
        public string UniversalFull { get; set; }
    }
}
