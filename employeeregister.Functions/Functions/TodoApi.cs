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
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable registerTable,
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
            await registerTable.ExecuteAsync(addOperation);
            string message = "New register stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(UpdateRegister))]
        public static async Task<IActionResult> UpdateRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "register/{id}")] HttpRequest req,
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log
            )
        {
            log.LogInformation($"update for register {id}, received");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Register todo = JsonConvert.DeserializeObject<Register>(requestBody);

            // validate register

            TableOperation findOperation = TableOperation.Retrieve<RegisterEntity>("TODO", id);
            TableResult findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Register not found"
                });

            }

            // update register

            RegisterEntity todoentity = (RegisterEntity)findResult.Result;
            todoentity.consolidated = todo.consolidated;
            todoentity.Type = todo.Type;

            if (!string.IsNullOrEmpty(todo.IdEmployee))
            {
                todoentity.IdEmployee = todo.IdEmployee;
            }

            TableOperation addOperation = TableOperation.Replace(todoentity);
            await todoTable.ExecuteAsync(addOperation);

            string message = $"register: {id}, update in table. ";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoentity
            });

        }

        [FunctionName(nameof(GetAllRegister))]
        public static async Task<IActionResult> GetAllRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "register")] HttpRequest req,
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log
            )
        {
            log.LogInformation("Get all register Received");
            TableQuery<RegisterEntity> query = new TableQuery<RegisterEntity>();
            TableQuerySegment<RegisterEntity> todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieve all todos";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todos
            });

        }


        [FunctionName(nameof(GetregisterByid))]
        public static IActionResult GetregisterByid(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "register/{id}")] HttpRequest req,
             [Table("Register", "TODO", "{id}", Connection = "AzureWebJobsStorage")] RegisterEntity todoentity,
             string id,
             ILogger log
            )
        {
            log.LogInformation($"get register by id: {id} received");

            if (todoentity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "register not found"
                });
            }

            string message = $"Todo: {id} retrieved. ";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoentity
            });
        }

        [FunctionName(nameof(DeleteRegister))]

        public static async Task<IActionResult> DeleteRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "register/{id}")] HttpRequest req,
            [Table("register", "TODO", "{id}", Connection = "AzureWebJobsStorage")] RegisterEntity todoentity,
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log
             )
        {
            log.LogInformation($"delete todo: {id} received");

            if (todoentity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Register not found"
                });
            }

            await todoTable.ExecuteAsync(TableOperation.Delete(todoentity));
            string message = $"Register:{id}, deleted";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoentity
            });
        }

    }
}
