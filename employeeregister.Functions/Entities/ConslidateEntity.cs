using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace employeeregister.Functions.Entities
{
    public class ConslidateEntity : TableEntity
    {
        public string IdEmployee { get; set; }
        public DateTime Date { get; set; }
        public int minutes { get; set; }
    }
}
