using DataMerger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DataMerger.Services
{
    public class DeserializeXML
    {
        private List<XmlUser> xmlUsers;
        public List<XmlUser> UniqueXmlUsers { get; private set; }
        public List<XmlUser> IncorrectData { get; private set; }       
        public void DeserializeUsersFromXML(string mainDirectory)
        {
            string xmlDirectory = Path.Combine(mainDirectory, "phone.xml");
            XDocument usersXml = XDocument.Load(new StreamReader(xmlDirectory));
            var users = usersXml.Root.Elements();
            xmlUsers = new List<XmlUser>();
            IncorrectData = new List<XmlUser>();

            foreach(var u in users)
            {
                InsertUserToRepo(u.Elements().ToList());
            }
           
            RemoveDuplicatesAndEmpties(xmlUsers);
        }
        public void RemoveDuplicatesAndEmpties(List<XmlUser> xmlUsers)
        {
            UniqueXmlUsers = new List<XmlUser>();

            foreach (var u in xmlUsers)
            {                
                var duplicates = xmlUsers.Where(n => (n.Username == u.Username) &&
                (!UniqueXmlUsers.Any(e=>e.Username == u.Username)))
                    .ToList();

                if(duplicates.Count() > 0)
                {
                    XmlUser uniqueUser = new XmlUser();
                    uniqueUser.Id = duplicates[0].Id;
                    uniqueUser.Username = duplicates[0].Username;
                    uniqueUser.Phones = new List<Phone>();

                    foreach (var d in duplicates)
                    {
                        Phone phone = d.Phones[0];
                        uniqueUser.Phones.Add(phone);
                    }

                    UniqueXmlUsers.Add(uniqueUser);
                }
            }

        }
        public void InsertUserToRepo(List<XElement> accountAttributes)
        {
            bool isCorrect = false;
            Phone phone = new Phone();
            XmlUser xmlUser = new XmlUser();
            List<Phone> phones = new List<Phone>();
            xmlUser.Phones = new List<Phone>();

            foreach (var a in accountAttributes)
            {
                switch (a.FirstAttribute.Value)
                {
                    case "id":
                        xmlUser.Id = a.Value;
                        break;
                    case "username":
                        xmlUser.Username = a.Value;
                        break;
                    case "phoneType":
                        phone.Type = a.Value;
                        break;
                    case "value":
                        phone.PhoneNumber = FormatPhoneNumber(a.Value, ref isCorrect);
                        break;
                }
            }
            xmlUser.Phones.Add(phone);

            if (isCorrect)
            {
                xmlUsers.Add(xmlUser);
            }
            else
            {
                IncorrectData.Add(xmlUser);
            }
                
        }
        public string FormatPhoneNumber(string number, ref bool isCorrect)
        {
            string formatedPhNumber = number;
            number = Regex.Replace(number, @"[^0-9]+", "");

            if (!String.IsNullOrEmpty(number) && number.Length == 10)
            {
                isCorrect = true;
                formatedPhNumber = number.Substring(0, 3) + " " + number.Substring(3, 3) + " " + number.Substring(6, 4);
            }
            return formatedPhNumber;
        }
    }
}
