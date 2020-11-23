using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataMerger.Models
{
    public class XmlUser
    {
        public XmlUser() { }

        public string Id { get; set; }
        public string Username { get; set; }
        public List<Phone> Phones { get; set; }
            
    }
    //if user has more than one phone number in .xml
    public class Phone
    {
        public string Type { get; set; }
        public string PhoneNumber { get; set; }
    }
}
