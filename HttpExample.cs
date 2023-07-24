using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace My.Functions
{
    public static class HttpExample
    {
        [FunctionName("HttpExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "visitorcount-db",
                collectionName: "visitorcount",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "count",
                PartitionKey = "count")] IAsyncCollector<dynamic> documentsOut,
            [CosmosDB(
                databaseName: "visitorcount-db",
                collectionName: "visitorcount",
                ConnectionStringSetting = "CosmosDbConnectionString",
                Id = "count",
                PartitionKey = "count")] dynamic document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (document == null)
            {
                // If the count entry does not exist, create one.
                document = new { id = "count", count = 1 };
            }
            else
            {
                // If the count entry exists, increase the count by 1.
                document.count += 1;
            }

            // Save the updated count to the database.
            await documentsOut.AddAsync(document);

            string responseMessage = $"{document.count}";

            return new OkObjectResult(responseMessage);
        }
    }
}
