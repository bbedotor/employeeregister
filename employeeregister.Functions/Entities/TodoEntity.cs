using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace employeeregister.Functions.Entities
{
    public class TodoEntity : TableEntity
    {
        public DateTime createTime { get; set; }
        public int Type { get; set; }
        public bool consolidated { get; set; }
    }
}
