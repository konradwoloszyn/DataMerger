using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMerger.Models
{
    public class FulltimeUser
    {
        public FulltimeUser(CsvUser csvUser, XmlUser xmlUser)
        {
            Employee_type = csvUser.Employee_type;
            Id = csvUser.Id;
            First_name = csvUser.First_name;
            Last_name = csvUser.Last_name;
            Email = csvUser.Email;
            EndDate = csvUser.EndDate;
            Gender = csvUser.Gender;
            Ext1 = csvUser.Ext1;
            Username = xmlUser.Username;
            Phones = xmlUser.Phones;
        }
        public string Employee_type { get; set; }
        public string Id { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EndDate { get; set; }
        public string Ext1 { get; set; }
        public string Username { get; set; }
        public List<Phone> Phones { get; set; }
    }
    public class ContractorUser
    {
        public ContractorUser(CsvUser csvUser, XmlUser xmlUser)
        {
            Employee_type = csvUser.Employee_type;
            First_name = csvUser.First_name;
            Last_name = csvUser.Last_name;
            Email = csvUser.Email;
            Gender = csvUser.Gender;
            EndDate = csvUser.EndDate;
            Ext1 = csvUser.Ext1;
            Username = xmlUser.Username;
            Phones = xmlUser.Phones;
        }
        public string Employee_type { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EndDate { get; set; }
        public string Ext1 { get; set; }
        public string Username { get; set; }
        public List<Phone> Phones { get; set; }
    }
    public class UserMergedData
    {
        public List<ContractorUser> ContractorUsers { get; set; }
        public List<FulltimeUser> FulltimeUsers { get; set; }
        public List<XmlUser> ContractorDataMissmatches { get; set; }
        public List<XmlUser> FulltimeDataMissmatches { get; set; }
        public List<ContractorUser> ContractorsOutDated { get; set; }
        public List<FulltimeUser> FulltimersOutDated { get; set; }
        public void MergeContractorUsers(List<XmlUser> xmlUsers, List<CsvUser> csvUsers)
        {
            ContractorUsers = new List<ContractorUser>();
            ContractorsOutDated = new List<ContractorUser>();
            DateTime minDate = Convert.ToDateTime("01.01.2019", System.Globalization.CultureInfo
                .GetCultureInfo("hi-IN").DateTimeFormat);

            foreach (var u in csvUsers)
            {
                if (u.Employee_type.Equals("Contractor", StringComparison.OrdinalIgnoreCase))
                {
                    string currentUsername = ExtractUsernameFromEmail(u.Email);
                    var xmlUser = xmlUsers
                        .Where(l => l.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (!(xmlUser is null))
                    {
                        var mergedUser = new ContractorUser(u, xmlUser);
                        DateTime currentEndDate = Convert.ToDateTime(mergedUser.EndDate, System.Globalization.CultureInfo
                        .GetCultureInfo("hi-IN").DateTimeFormat);

                        if (currentEndDate.Date > minDate.Date)
                            ContractorUsers.Add(mergedUser);
                        else
                            ContractorsOutDated.Add(mergedUser);
                    }

                }

            }
            DataMissMatchesForContractor(xmlUsers);
        }
        public void MergeFulltimeUsers(List<XmlUser> xmlUsers, List<CsvUser> csvUsers)
        {
            FulltimeUsers = new List<FulltimeUser>();
            FulltimersOutDated = new List<FulltimeUser>();
            DateTime minDate = Convert.ToDateTime("01.01.2019", System.Globalization.CultureInfo
                 .GetCultureInfo("hi-IN").DateTimeFormat);

            foreach (var u in csvUsers)
            {
                if (!String.IsNullOrEmpty(u.Id))
                {
                    string currentId = Encode(u.Id);
                    var xmlUser = xmlUsers
                        .Where(i => (i.Id == currentId))
                        .FirstOrDefault();

                        if (!(xmlUser is null))
                        {
                            var mergedUser = new FulltimeUser(u, xmlUser);
                            DateTime currentEndDate = Convert.ToDateTime(mergedUser.EndDate, System.Globalization.CultureInfo
                            .GetCultureInfo("hi-IN").DateTimeFormat);

                            if (currentEndDate.Date > minDate.Date)
                                FulltimeUsers.Add(new FulltimeUser(u, xmlUser));
                            else
                                FulltimersOutDated.Add(mergedUser);
                        }
                }
            }
            DataMissMatchesForFulltime(xmlUsers);
        }
        public void DataMissMatchesForContractor(List<XmlUser> xmlUsers)
        {
            List<string> usernames = ContractorUsers
                .Select(u => u.Username)
                .ToList();

            List<string> contractorFromOutDated = ContractorsOutDated
                .Select(u => u.Username)
                .ToList();

            usernames = usernames.Concat(contractorFromOutDated).ToList(); 

            ContractorDataMissmatches = xmlUsers
                .Where(u => !usernames
                .Contains(u.Username) &&
                (String.IsNullOrEmpty(u.Id)))
                .ToList();
        }
        public void DataMissMatchesForFulltime(List<XmlUser> xmlUsers)
        {
            List<string> usernames = FulltimeUsers
                .Select(u => Encode(u.Id))
                .ToList();

            List<string> fullTimersFromOutDated = FulltimersOutDated
                .Select(u => Encode(u.Id))
                .ToList();

            usernames = usernames.Concat(fullTimersFromOutDated).ToList();

            FulltimeDataMissmatches = xmlUsers
                .Where(u => (!String.IsNullOrEmpty(u.Id)) &&
                !usernames
                .Contains(u.Id))
                .ToList();
        }
        public string Encode(string id)
        {
            Guid guid = new Guid(id);
            string encodedGuid = Convert.ToBase64String(guid.ToByteArray());
            return encodedGuid;
        }
        public string ExtractUsernameFromEmail(string email) =>
            email.Split('@')[0];
    }
}
