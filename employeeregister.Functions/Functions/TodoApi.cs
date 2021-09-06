using employeeregister.Common.Models;
using employeeregister.Common.Responses;
using employeeregister.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace employeeregister.Functions.Functions
{
    public static class TodoAPI
    {
        [FunctionName(nameof(CreateEentry))]
        public static async Task<IActionResult> CreateEentry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequest req,
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new entry");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Register todo = JsonConvert.DeserializeObject<Register>(requestBody);

            if (string.IsNullOrEmpty(todo?.IdEmployee))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "the request must have a Employee"
                });
            }

            RegisterEntity todoEntity = new RegisterEntity
            {
                createTime = DateTime.UtcNow,
                ETag = "*",
                consolidated = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = todo.IdEmployee,
                Type = todo.Type
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);
            string message = "New register stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
    }
}
