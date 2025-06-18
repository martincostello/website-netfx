// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientWrapper.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   HttpClientWrapper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// A class representing an implementation of <see cref="IHttpClient"/> that wraps an instance of <see cref="HttpClient"/>.
    /// </summary>
    public class HttpClientWrapper : IHttpClient
    {
        /// <summary>
        /// The default <c>Accept</c> HTTP header to specify.  This field is read-only.
        /// </summary>
        private static readonly MediaTypeWithQualityHeaderValue DefaultAcceptHeader = new MediaTypeWithQualityHeaderValue("application/json");

        /// <summary>
        /// The default <c>UserAgent</c> HTTP header to specify.
        /// </summary>
        private static readonly ProductInfoHeaderValue DefaultUserAgentHeader = CreateUserAgentHeader();

        /// <summary>
        /// The <see cref="HttpClient"/> wrapped by this instance.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        public HttpClientWrapper()
            : this(baseAddress: null, setAcceptHeader: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="baseAddress">The base URI to use.</param>
        public HttpClientWrapper(Uri baseAddress)
            : this(baseAddress, setAcceptHeader: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="baseAddress">The base URI to use, if any.</param>
        /// <param name="setAcceptHeader">Whether to set a default value for the <c>Accept</c> HTTP request header.</param>
        private HttpClientWrapper(Uri baseAddress, bool setAcceptHeader)
        {
            _client = new HttpClient();

            try
            {
                _client.BaseAddress = baseAddress;

                if (setAcceptHeader)
                {
                    _client.DefaultRequestHeaders.Accept.Add(DefaultAcceptHeader);
                }

                _client.DefaultRequestHeaders.UserAgent.Add(DefaultUserAgentHeader);
            }
            catch (Exception)
            {
                _client.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        ~HttpClientWrapper()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the HTTP <c>User-Agent</c> header in use by the application.
        /// </summary>
        public static string UserAgentHeader
        {
            get { return DefaultUserAgentHeader.ToString(); }
        }

        /// <summary>
        /// Gets or sets the Authorization HTTP header.
        /// </summary>
        public AuthenticationHeaderValue Authorization
        {
            get { return this._client.DefaultRequestHeaders.Authorization; }
            set { this._client.DefaultRequestHeaders.Authorization = value; }
        }

        /// <summary>
        /// Gets or sets the base URI in use by the instance.
        /// </summary>
        public Uri BaseAddress
        {
            get { return this._client.BaseAddress; }
            set { this._client.BaseAddress = value; }
        }

        /// <summary>
        /// Gets the default HTTP request headers.
        /// </summary>
        public HttpHeaders DefaultRequestHeaders
        {
            get { return this._client.DefaultRequestHeaders; }
        }

        /// <summary>
        /// Gets or sets the period to wait before a request times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return this._client.Timeout; }
            set { this._client.Timeout = value; }
        }

        /// <inheritdoc />
        public virtual async Task<HttpResponseMessage> GetAsync(string requestPath)
        {
            return await _client.GetAsync(requestPath);
        }

        /// <inheritdoc />
        public virtual async Task<HttpResponseMessage> PostAsync(string requestPath, HttpContent content)
        {
            return await _client.PostAsync(requestPath, content);
        }

        /// <inheritdoc />
        public virtual async Task<HttpResponseMessage> PostAsJsonAsync(string requestPath, object value)
        {
            return await _client.PostAsJsonAsync(requestPath, value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and, optionally, managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        _client.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Creates the default instance of <see cref="ProductInfoHeaderValue"/> to use.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="ProductInfoHeaderValue"/>.
        /// </returns>
        private static ProductInfoHeaderValue CreateUserAgentHeader()
        {
            Assembly assembly = typeof(MvcApplication).Assembly;
            string productVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            return new ProductInfoHeaderValue("martincostello.com", productVersion);
        }
    }
}