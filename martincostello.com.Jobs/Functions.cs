// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Functions.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   Functions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace MartinCostello.Root.Jobs
{
    /// <summary>
    /// A class containing functions for use by Azure Web Jobs. This class cannot be inherited.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Sends a notification that the fish need feeding if they have not been fed.
        /// </summary>
        /// <param name="log">The <see cref="TextWriter"/> to use for logging.</param>
        /// <param name="connectionString">The Azure Storage connection string to use.</param>
        [NoAutomaticTrigger]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "It is not disposed of multiple times.")]
        public static void SendFishFeedingNotification(TextWriter log, string connectionString)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();            
            CloudBlobContainer container = blobClient.GetContainerReference("fish");
            CloudBlockBlob blob = container.GetBlockBlobReference("fish.json");

            string json;

            using (Stream stream = blob.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            }

            dynamic fishData = JObject.Parse(json);

            List<DateTime> feedingDates = new List<DateTime>();

            foreach (dynamic feeding in fishData.feedingDates as JArray)
            {
                DateTime timestamp = feeding.timestamp;
                feedingDates.Add(timestamp);
            }

            DateTime lastFeeding = feedingDates
                .OrderByDescending((p) => p)
                .FirstOrDefault();

            DateTime now = DateTime.UtcNow;
            bool isFeedingOverdue = false;

            // Have the fish been fed by 10pm?
            if (lastFeeding.Date < now.Date && now.Hour > 21)
            {
                isFeedingOverdue = true;
            }

            if (isFeedingOverdue)
            {
                // TODO Get the emails and/or Twitter handles of all the users with
                // the fish role and send out a notification/reminder
                ////CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                ////tableClient.GetTableReference("");
            }
        }
    }
}
