using System;
using System.Threading.Tasks;
using employeeregister.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace employeeregister.Functions.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("register", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"consolidate completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("consolidated", QueryComparisons.Equal, false);

            TableQuery<RegisterEntity> query = new TableQuery<RegisterEntity>().Where(filter);
            TableQuerySegment<RegisterEntity> completedTodos = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int replace = 0;


            foreach (RegisterEntity completedTodo in completedTodos)
            {
                if (!completedTodo.consolidated == false)
                {
                    completedTodo.consolidated = true;
                }
                TableOperation addOperation = TableOperation.Replace(completedTodo);
                await todoTable.ExecuteAsync(addOperation);
                replace++;
            }
                
            log.LogInformation($"consolidate: {replace} items at: {DateTime.Now}");
        }
    }
}
