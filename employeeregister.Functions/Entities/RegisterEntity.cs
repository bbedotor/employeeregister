using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace employeeregister.Functions.Entities
{
    public class RegisterEntity : TableEntity
    {
        public DateTime createTime { get; set; }
        public string IdEmployee { get; set; }
        public int Type { get; set; }
        public bool consolidated { get; set; }
    }
}
