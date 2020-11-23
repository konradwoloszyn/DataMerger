using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataMerger.Models
{
    public class CsvUser
    {

        public CsvUser() { }

        public string Employee_type { get; set; }
        public string Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EndDate { get; set; }
        public string Ext1 { get; set; }
    }
}
