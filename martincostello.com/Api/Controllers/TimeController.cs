// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   TimeController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Api.Controllers
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Description;
    using MartinCostello.Api.Models;

    /// <summary>
    /// Gets the current time.
    /// </summary>
    public class TimeController : ApiControllerBase
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="TimeResponse"/> containing the current time.
        /// </returns>
        [EnableCors(origins: "*", headers: "*", methods: "get")]
        [HttpGet]
        [ResponseType(typeof(TimeResponse))]
        public async Task<HttpResponseMessage> Get(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var now = DateTime.UtcNow;

                var body = new TimeResponse()
                {
                    Rfc1123 = now.ToString("r", CultureInfo.InvariantCulture),
                    UniversalFull = now.ToString("U", CultureInfo.InvariantCulture),
                    UniversalSortable = now.ToString("u", CultureInfo.InvariantCulture),
                    Unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                };

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, body));
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to generate status response for request Id '{0}': {1}", RequestId, ex.Message);
            }
        }
    }
}
