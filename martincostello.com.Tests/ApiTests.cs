// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiTests.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   ApiTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class containing tests for the API.
    /// </summary>
    [TestClass]
    public class ApiTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTests"/> class.
        /// </summary>
        public ApiTests()
        {
        }

        [TestMethod]
        [TestCategory("API")]
        [TestCategory("Integration")]
        [Description("Tests the /time REST API resource.")]
        public async Task Api_Get_Time_Returns_Correct_Response()
        {
            // Arrange
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBaseUri"], UriKind.Absolute);

                DateTime now = DateTime.Now;

                // Act
                using (HttpResponseMessage response = await client.GetAsync("time"))
                {
                    // Assert
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The HTTP status code is incorrect.");

                    string json = await response.Content.ReadAsStringAsync();
                    dynamic value = Newtonsoft.Json.Linq.JObject.Parse(json);

                    string rfc1123 = (string)value.rfc1123;
                    long unix = (long)value.unix;
                    string universalSortable = (string)value.universalSortable;
                    string universalFull = (string)value.universalFull;

                    Assert.IsTrue(DateTime.TryParseExact(rfc1123, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var rfc1123Value), "Failed to parse value '{0}'.", rfc1123);
                    Assert.IsTrue(DateTime.TryParseExact(universalSortable, "u", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var universalSortableValue), "Failed to parse value '{0}'.", universalSortable);
                    Assert.IsTrue(DateTime.TryParseExact(universalFull, "U", CultureInfo.InvariantCulture, DateTimeStyles.None, out var universalFullValue), "Failed to parse value '{0}'.", universalFull);

                    Assert.AreEqual(rfc1123Value, universalSortableValue, "RFC1123 value {0:u} is not equal to Universal Sortable value {1:u}.", rfc1123Value, universalSortableValue);
                    Assert.AreEqual(rfc1123Value, universalFullValue, "RFC1123 value {0:u} is not equal to Universal Full value {1:u}.", rfc1123Value, universalFullValue);

                    // Value is allowed to be 5 minnutes either side of local time
                    Assert.IsTrue(rfc1123Value <= now.AddMinutes(5));
                    Assert.IsTrue(rfc1123Value >= now.AddMinutes(-5));

                    Assert.AreEqual("utf-8", response.Content.Headers.ContentType.CharSet, "The response content type character set is incorrect.");
                    Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType, "The response content type media type is incorrect.");

                    var expectedHeaders = new Tuple<string, string>[]
                    {
                        Tuple.Create("Strict-Transport-Security", "max-age=31536000"),
                        Tuple.Create("X-Content-Type-Options", "nosniff"),
                        Tuple.Create("X-Frame-Options", "DENY"),
                        Tuple.Create("X-XSS-Protection", "1; mode=block"),
                    };

                    foreach (var testCase in expectedHeaders)
                    {
                        Assert.AreEqual(testCase.Item2, response.Headers.GetValues(testCase.Item1).FirstOrDefault(), "The value of the '{0}' header is incorrect.", testCase.Item1);
                    }

                    var expectedHeaders2 = new string[]
                    {
                        "X-Instance",
                        "X-Request-Duration",
                        "X-Request-Id",
                    };

                    foreach (var testCase in expectedHeaders2)
                    {
                        Assert.IsFalse(
                            string.IsNullOrWhiteSpace(response.Headers.GetValues(testCase).FirstOrDefault()),
                            "The '{0}' header was not returned.",
                            testCase);
                    }
                }
            }
        }
    }
}
