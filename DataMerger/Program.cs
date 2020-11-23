using DataMerger.Models;
using DataMerger.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            string mainPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            Console.WriteLine("Please insert path to folder where you store data files:");
            string filesLocation = Console.ReadLine();


            if (File.Exists(filesLocation+"\\phone.xml") && File.Exists(filesLocation+"\\users.csv"))
            {
                DeserializeXML deserializeXML = new DeserializeXML();
                deserializeXML.DeserializeUsersFromXML(filesLocation);
                DeserializeCSV deserializeCSV = new DeserializeCSV();
                deserializeCSV.DeserializeUsersFromCSV(filesLocation);
                UserMergedData userMergedData = new UserMergedData();

                userMergedData.MergeContractorUsers(deserializeXML.UniqueXmlUsers,
                    deserializeCSV.UniqueCsvUsers);

                userMergedData.MergeFulltimeUsers(deserializeXML.UniqueXmlUsers,
                    deserializeCSV.UniqueCsvUsers);

                ReportGenerator reportGenerator = new ReportGenerator(userMergedData, deserializeXML, deserializeCSV);
                reportGenerator.GenerateOutputs(mainPath);

                Console.WriteLine($"Operation success, check output files in: {mainPath}\\Output files");
            }
            else
            {
                Console.WriteLine("Operation failed, wrong path, file or files doesn't exist in the directory which was pointed");
            }

            Console.ReadKey();
        }
    }
}
