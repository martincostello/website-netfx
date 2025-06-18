// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   Program.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Configuration;
using Microsoft.Azure.WebJobs;

namespace MartinCostello.Root.Jobs
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    internal static class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        internal static void Main()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString;

            JobHostConfiguration config = new JobHostConfiguration()
            {
                DashboardConnectionString = connectionString,
                StorageConnectionString = connectionString,
            };

            using (JobHost host = new JobHost(config))
            {
                // The following code will invoke a function called ManualTrigger and 
                // pass in data (value in this case) to the function
                host.Call(typeof(Functions).GetMethod("SendFishFeedingNotification"), new { connectionString = connectionString });
            }
        }
    }
}
