using DataMerger.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMerger.Services
{
    public class ReportGenerator
    {
        private UserMergedData userMergedData;
        private DeserializeXML deserializeXML;
        private DeserializeCSV deserializeCSV;

        public ReportGenerator(UserMergedData userMergedData, DeserializeXML deserializeXML, DeserializeCSV deserializeCSV)
        {
            this.userMergedData = userMergedData;
            this.deserializeXML = deserializeXML;
            this.deserializeCSV = deserializeCSV;
        }

        public void GenerateOutputs(string mainPath)
        {
            string contractorRaportPath = Path.Combine(mainPath, "Output files", "ContractorUsers.txt");
            string fulltimersRaportPath = Path.Combine(mainPath, "Output files", "FulltimeUsers.txt");
            string errorsReport = Path.Combine(mainPath, "Output files", "Report.txt");

            GenerateJsonFile(contractorRaportPath, "contractor");
            GenerateJsonFile(fulltimersRaportPath, "fulltimer");
            GenerateErrorsReport(errorsReport);
        }
        private void GenerateJsonFile(string path, string mode)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                if(mode.Equals("contractor"))
                    jsonSerializer.Serialize(file, userMergedData.ContractorUsers);
                else
                    jsonSerializer.Serialize(file, userMergedData.FulltimeUsers);
            }
        } 
        private void GenerateErrorsReport(string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                file.WriteLine("Incorrect phone number format(" + deserializeXML.IncorrectData.Count()+").");
                file.WriteLine("Empty date fields(" + deserializeCSV.DateEmpty.Count() + ").");
                file.WriteLine("Empty or invalid email address(" + deserializeCSV.IncorrectEmail.Count() + ").");
                file.WriteLine("Date and email are incorrect(" + deserializeCSV.IncorrectEmailAndDate.Count() + ").");
                file.WriteLine("Contractors out of date range(" + userMergedData.ContractorsOutDated.Count() + ").");
                file.WriteLine("Fulltimers out of date range(" + userMergedData.FulltimersOutDated.Count() + ").");
                file.WriteLine("There is no corresponding records in users.csv for Contractors(" + 
                    userMergedData.ContractorDataMissmatches.Count() + ").");
                file.WriteLine("There is no corresponding records in users.csv for Fulltimers(" + 
                    userMergedData.FulltimeDataMissmatches.Count() + ").");

            }
        }
    }
}
