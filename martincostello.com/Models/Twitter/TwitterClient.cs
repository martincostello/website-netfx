// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwitterClient.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   TwitterClient.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// A class representing a client for posting tweets to Twitter.
    /// </summary>
    public class TwitterClient
    {
        /// <summary>
        /// The maximum length of a tweet in characters.
        /// </summary>
        internal const int MaxTweetLength = 140;

        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(TwitterClient));

        /// <summary>
        /// The base URI of the Twitter API endpoint.
        /// </summary>
        private static readonly Uri ApiEndpoint = new Uri("https://api.twitter.com/");

        /// <summary>
        /// The base URI of the Twitter upload endpoint.
        /// </summary>
        private static readonly Uri UploadEndpoint = new Uri("https://upload.twitter.com/");

        /// <summary>
        /// An empty instance of <see cref="NameValueCollection"/>.
        /// </summary>
        /// <remarks>
        /// <c>OAuth</c> for uploading media requires no parameters in the signature.
        /// </remarks>
        private static readonly NameValueCollection EmptyParameters = new NameValueCollection();

        /// <summary>
        /// A delegate to a method to use to create an <see cref="IHttpClient"/> for URI.
        /// </summary>
        private readonly Func<Uri, IHttpClient> _clientFactory;

        /// <summary>
        /// The <c>OAuth</c> keys to use.
        /// </summary>
        private readonly IOAuthKeys _keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterClient"/> class.
        /// </summary>
        /// <param name="keys">The <c>OAuth</c> keys to use for authentication.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keys"/> is <see langword="null"/>.
        /// </exception>
        public TwitterClient(IOAuthKeys keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            _clientFactory = (p) => p == null ? new HttpClientWrapper() : new HttpClientWrapper(p);
            _keys = keys;
        }

        /// <summary>
        /// Posts the specified content to Twitter asynchronously.
        /// </summary>
        /// <param name="tweet">The content to tweet.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation to post the tweet which returns the ID of the posted tweet.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tweet"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="tweet"/> contains no body to tweet.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Not an issue for async methods.")]
        public async Task<long?> PostAsync(string tweet)
        {
            return await PostAsync(tweet, null);
        }

        /// <summary>
        /// Posts the specified content to Twitter asynchronously.
        /// </summary>
        /// <param name="tweet">The content to tweet.</param>
        /// <param name="imageUri">The optional URI of an image to post with the tweet.</param>
        /// <returns>
        /// A <see cref="Task" /> representing the asynchronous operation to post the tweet which returns the ID of the posted tweet.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tweet"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="tweet"/> contains no body to tweet.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Not an issue for async methods.")]
        public async Task<long?> PostAsync(string tweet, Uri imageUri)
        {
            if (tweet == null)
            {
                throw new ArgumentNullException("tweet");
            }

            long? tweetId = null;

            if (string.IsNullOrWhiteSpace(tweet))
            {
                throw new ArgumentException("No body to tweet was specified.", "tweet");
            }

            // Remove any extraneous whitespace as this is wasteful towards the character limit
            tweet = tweet.Trim();

            // Automatically truncate the content if it is too long to be tweeted
            if (tweet.Length > MaxTweetLength)
            {
                tweet = tweet.Substring(0, MaxTweetLength - 3) + "...";
            }

            NameValueCollection parameters = new NameValueCollection()
            {
                { "status", tweet },
            };

            if (imageUri != null)
            {
                string mediaId = await UploadImageAsync(imageUri);

                if (!string.IsNullOrWhiteSpace(mediaId))
                {
                    parameters["media_ids"] = mediaId;
                }
            }

            using (IHttpClient client = _clientFactory(ApiEndpoint))
            {
                const string Path = "/1.1/statuses/update.json";

                ApplyOAuthSignature(client, Path, HttpMethod.Post.Method, parameters);

                List<KeyValuePair<string, string>> nameValueCollection = new List<KeyValuePair<string, string>>();

                // OAuth requires the parameters to be sorted alphabetically, so the parameters in the form should match
                foreach (string name in parameters.AllKeys.OrderBy((p) => p, StringComparer.Ordinal))
                {
                    nameValueCollection.Add(new KeyValuePair<string, string>(name, parameters[name]));
                }

                // Twitter can be tempramental and sometimes fail to authenticate the request and the post fails.
                // Adding in some retry logic to try and determine whether this is just the way Twitter is, or if
                // there is a subtle problem in the code. Attempting to repost the same content that failed at one
                // point with the same API keys at a later date will succeed. Either there's a subtle timing bug
                // (maybe clock-skew?) or Twitter just randomly decides to reject API requests, which would be odd.
                const int MaximumPostAttempts = 5;
                TimeSpan pause = TimeSpan.FromSeconds(1);

                for (int i = 0; i < MaximumPostAttempts; i++)
                {
                    using (HttpContent httpContent = new FormUrlEncodedContent(nameValueCollection))
                    {
                        using (HttpResponseMessage result = await client.PostAsync(Path, httpContent))
                        {
                            Log.DebugFormat(CultureInfo.InvariantCulture, "POST of tweet to '{0}' returned HTTP status code {1}.", Path, (int)result.StatusCode);

                            string response = string.Empty;

                            if (result.Content != null)
                            {
                                // Read the response now, as reading it later if there's an error generates an ObjectDisposedException
                                response = result.Content.ReadAsStringAsync().Result;
                            }

                            if (result.IsSuccessStatusCode)
                            {
                                // Parse the posted tweet and return its Id
                                dynamic tweetResponse = JObject.Parse(response);
                                tweetId = (long)tweetResponse.id;

                                break;
                            }

                            Log.WarnFormat(CultureInfo.InvariantCulture, "Tweet response body: '{0}'", response);

                            if (i > MaximumPostAttempts - 2)
                            {
                                // Final attempt, so thow the exception out to the caller and give up
                                result.EnsureSuccessStatusCode();
                            }
                            else
                            {
                                // Wait until the next attempt to prevent possible rate-limiting issues (except on the last attempt)
                                await Task.Delay(pause);
                            }
                        }
                    }
                }
            }

            return tweetId;
        }

        /// <summary>
        /// Asynchronously uploads the image at the specified URI to Twitter.
        /// </summary>
        /// <param name="imageUri">The URI of the image to upload.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> that represents the asynchronous task to
        /// upload the image to Twitter and return the media Id associated with it.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="imageUri"/> is not an absolute URI.
        /// </exception>
        internal virtual async Task<string> UploadImageAsync(Uri imageUri)
        {
            Debug.Assert(imageUri != null, "uri is null.");

            if (!imageUri.IsAbsoluteUri)
            {
                throw new ArgumentException("The specified image URI is not an absolute URI.", "imageUri");
            }

            ImageData imageData;

            // Do not specify a base address so that a custom 'Accept' HTTP request header is not sent
            using (IHttpClient client = _clientFactory(null))
            {
                imageData = await DownloadImageAsync(client, imageUri);
            }

            if (imageData == null)
            {
                return null;
            }

            using (IHttpClient client = _clientFactory(UploadEndpoint))
            {
                Debug.Assert(imageData.Buffer != null, "ImageData.Buffer is null.");

                const int MaxImageLength = 3145728; // 3MB

                if (imageData.Buffer.Length > MaxImageLength)
                {
                    return null;
                }

                const string Path = "/1.1/media/upload.json";

                // Images are posted as multi-part form data, so are not included in the OAuth parameters
                ApplyOAuthSignature(client, Path, HttpMethod.Post.Method, EmptyParameters);

                using (MultipartFormDataContent formContent = new MultipartFormDataContent())
                {
                    using (HttpContent byteContent = new ByteArrayContent(imageData.Buffer))
                    {
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue(imageData.MediaType);
                        formContent.Add(byteContent, "media");

                        using (var response = await client.PostAsync(Path, formContent))
                        {
                            Debug.Assert(response != null, "HttpResponseMessage is null.");

                            if (!response.IsSuccessStatusCode)
                            {
                                Log.ErrorFormat(CultureInfo.InvariantCulture, "Failed to post image '{0}' to twitter. HTTP status code {1}.", imageUri, (int)response.StatusCode);
                                return null;
                            }

                            Debug.Assert(response.Content != null, "HttpResponseMessage.Content is null.");

                            string json = await response.Content.ReadAsStringAsync();

                            dynamic result = JObject.Parse(json);
                            return result.media_id;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tweet with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="id">The Id of the tweet to obtain.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing an asynchronous operation to get the tweet with the specified Id.
        /// </returns>
        internal async Task<dynamic> GetTweetAsync(long id)
        {
            NameValueCollection parameters = new NameValueCollection()
            {
                { "trim_user", "true" },
                { "include_entities", "false" },
            };

            using (IHttpClient client = _clientFactory(ApiEndpoint))
            {
                string path = string.Format(
                    CultureInfo.InvariantCulture,
                    "/1.1/statuses/show/{0}.json?trim_user=true&include_entities=false",
                    id);

                ApplyOAuthSignature(client, path, HttpMethod.Get.Method, parameters);

                using (HttpResponseMessage result = await client.GetAsync(path))
                {
                    Log.DebugFormat(CultureInfo.InvariantCulture, "GET request to '{0}' returned HTTP status code {1}.", path, (int)result.StatusCode);

                    result.EnsureSuccessStatusCode();

                    string response = await result.Content.ReadAsStringAsync();
                    return JObject.Parse(response);
                }
            }
        }

        /// <summary>
        /// Downloads the specified image asynchronously.
        /// </summary>
        /// <param name="client">The <see cref="IHttpClient"/> to use to download the image.</param>
        /// <param name="imageUri">The absolute URI of the image to download.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation to download the image.
        /// </returns>
        private static async Task<ImageData> DownloadImageAsync(IHttpClient client, Uri imageUri)
        {
            byte[] buffer;
            string mediaType;

            // Get the raw data of the image to upload and its media type
            using (var response = await client.GetAsync(imageUri.AbsoluteUri))
            {
                Debug.Assert(response != null, "HttpResponseMessage is null.");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                Debug.Assert(response.Content != null, "HttpResponseMessage.Content is null.");
                buffer = await response.Content.ReadAsByteArrayAsync();

                if (response.Content.Headers.ContentType == null)
                {
                    mediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
                }
                else
                {
                    mediaType = response.Content.Headers.ContentType.MediaType;
                }
            }

            return new ImageData()
            {
                Buffer = buffer,
                MediaType = mediaType,
            };
        }

        /// <summary>
        /// Applies an <c>OAuth</c> signature to the specified <see cref="IHttpClient"/>.
        /// </summary>
        /// <param name="client">The <see cref="IHttpClient"/> to apply the signature to.</param>
        /// <param name="path">The relative path of the request to authorize.</param>
        /// <param name="method">The HTTP method being performed for the request to <paramref name="path"/>.</param>
        /// <param name="parameters">The parameters for the HTTP request to <paramref name="path"/>.</param>
        private void ApplyOAuthSignature(IHttpClient client, string path, string method, NameValueCollection parameters)
        {
            // The OAuth header must contain the full URI
            Uri requestUri = new Uri(client.BaseAddress, path);

            string oauthHeader = OAuthHeaderGenerator.GenerateHeaderValue(
                method,
                requestUri,
                parameters,
                _keys);

            client.Authorization = new AuthenticationHeaderValue("OAuth", oauthHeader);
        }

        /// <summary>
        /// A class representing data about a downloading image. This class cannot be inherited.
        /// </summary>
        private sealed class ImageData
        {
            /// <summary>
            /// Gets or sets the buffer containing the image data.
            /// </summary>
            internal byte[] Buffer { get; set; }

            /// <summary>
            /// Gets or sets the media type of the image.
            /// </summary>
            internal string MediaType { get; set; }
        }
    }
}
