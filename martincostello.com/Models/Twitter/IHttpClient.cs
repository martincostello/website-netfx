// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpClient.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   IHttpClient.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// Defines a client for an HTTP API.
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Gets or sets the base address used by the client.
        /// </summary>
        Uri BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the Authorization HTTP header.
        /// </summary>
        AuthenticationHeaderValue Authorization { get; set; }

        /// <summary>
        /// Gets the default HTTP request headers.
        /// </summary>
        HttpHeaders DefaultRequestHeaders { get; }

        /// <summary>
        /// Gets or sets the period to wait before a request times out.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Performs an HTTP GET as an asynchronous operation.
        /// </summary>
        /// <param name="requestPath">The request URI.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation.
        /// </returns>
        Task<HttpResponseMessage> GetAsync(string requestPath);

        /// <summary>
        /// Performs an HTTP POST as an asynchronous operation.
        /// </summary>
        /// <param name="requestPath">The request URI.</param>
        /// <param name="content">The content to POST.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation.
        /// </returns>
        Task<HttpResponseMessage> PostAsync(string requestPath, HttpContent content);

        /// <summary>
        /// Sends a POST request as an asynchronous operation with the given value serialized as JSON.
        /// </summary>
        /// <param name="requestPath">The request URI.</param>
        /// <param name="value">The value that will be placed in the request's entity body.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation.
        /// </returns>
        Task<HttpResponseMessage> PostAsJsonAsync(string requestPath, object value);
    }
}