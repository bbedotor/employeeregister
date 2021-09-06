using System;

namespace employeeregister.Common.Models
{
    public class Register
    {
        public DateTime createTime { get; set; }
        public string IdEmployee { get; set; }
        public int Type { get; set; }
        public bool consolidated { get; set; }
    }
}
