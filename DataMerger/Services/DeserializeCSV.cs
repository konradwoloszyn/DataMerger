using DataMerger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataMerger.Services
{
    public class DeserializeCSV
    {
        public List<CsvUser> UniqueCsvUsers { get; private set; }
        public List<CsvUser> DateEmpty { get; private set; }
        public List<CsvUser> IncorrectEmail { get; private set; }
        public List<CsvUser> IncorrectEmailAndDate { get; private set; }
        public void DeserializeUsersFromCSV(string mainDirectory)
        {
            string csvDirectory = Path.Combine(mainDirectory, "users.csv");
            List<string> usersCsv = File.ReadLines(csvDirectory).ToList();
            usersCsv.RemoveAt(0);

            IncorrectEmailAndDate = new List<CsvUser>();
            IncorrectEmail = new List<CsvUser>();
            DateEmpty = new List<CsvUser>();
            UniqueCsvUsers = new List<CsvUser>();

            foreach (var u in usersCsv)
            {
                var temp = u.Replace(@"""", "");
                InsertCsvUsersToRepo(temp.Split(','));
            }

        }
        private void InsertCsvUsersToRepo(string[] users)
        {
            DateTime minDate = Convert.ToDateTime("01.01.2019", System.Globalization.CultureInfo
                 .GetCultureInfo("hi-IN").DateTimeFormat);
            bool isDateCorrect = false;
            bool isEmailCorrect = false;
            DateTime currentUserDate = new DateTime();

            CsvUser csvUser = new CsvUser();

            for (int i = 0; i < users.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        csvUser.Employee_type = users[i];
                        break;
                    case 1:
                        csvUser.Id = users[i];
                        break;
                    case 2:
                        csvUser.First_name = users[i];
                        break;
                    case 3:
                        csvUser.Last_name = users[i];
                        break;
                    case 4:
                        csvUser.Email = EmailValidation(users[i], ref isEmailCorrect);
                        break;
                    case 5:
                        csvUser.Gender = users[i];
                        break;
                    case 6:
                        csvUser.EndDate = ConvertToDate(users[i], ref isDateCorrect, ref currentUserDate);
                        break;
                    case 7:
                        csvUser.Ext1 = users[i];
                        break;
                }
            }

            if (!isEmailCorrect && !isDateCorrect)
            {
                IncorrectEmailAndDate.Add(csvUser);
            }
            else if (!isEmailCorrect)
            {
                IncorrectEmail.Add(csvUser);
            }
            else if (!isDateCorrect)
            {
                DateEmpty.Add(csvUser);
            }
            else
            {
                UniqueCsvUsers.Add(csvUser);
            }
        }

        //Check email format
        private string EmailValidation(string email, ref bool isEmailCorrect)
        {
            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            isEmailCorrect = Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);

            return email;
        }
        private string ConvertToDate(string date, ref bool isDateCorrect, ref DateTime currentUserDate)
        {
            DateTime formatedDate = new DateTime();
            string result = date;
            if (!String.IsNullOrEmpty(date))
            {
                formatedDate = Convert.ToDateTime(date, System.Globalization.CultureInfo
                        .GetCultureInfo("hi-IN").DateTimeFormat);
                isDateCorrect = true;
                result = formatedDate.Date.ToString("dd-MM-yyyy");
            }
            currentUserDate = formatedDate;

            return result;
        }
    }
}
