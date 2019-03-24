using System;
using System.ComponentModel.DataAnnotations;

namespace MShare_ASP.Data {
    public class Test {
        [Key]
        public Int32 IDTest { get; set; }
        public String  Name { get; set; }
    }
}
