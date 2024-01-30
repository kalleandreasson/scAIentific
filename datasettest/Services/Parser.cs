using datasettest.Models;
using ClosedXML.Excel;
using System.Collections.Generic;
using datasettest.Models;

namespace datasettest.Services;

public class Parser
{

    private readonly string folderPath;
    private readonly List<Report> reports;

    public Parser()
    {
        this.folderPath = "../datasettest/Data/Data.xlsx";
        reports = new List<Report>();
        ParseDataset();
    }

    public List<Report> ParseDataset()
    {

        using (var workbook = new XLWorkbook(folderPath))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed(); // Skip header row

            foreach (var row in rows)
            {
                if (row.RowNumber() == 1) // Assuming the first row contains the headers
                    continue;

                var report = new Report
                {
                     ArticleID = CleanString(row.Cell("A").GetValue<string>()),
                    Authors = CleanString(row.Cell("B").GetValue<string>()),
                    Title = CleanString(row.Cell("C").GetValue<string>()),
                    Year = row.Cell("D").GetValue<int>(), // Ensure this cell is formatted as an integer or use int.TryParse
                    Abstract = CleanString(row.Cell("E").GetValue<string>()),
                    FullReference = CleanString(row.Cell("F").GetValue<string>()),
                    // Add other fields if there are more columns
                };

                reports.Add(report);
            }
        }

        return reports;
    }

   private string CleanString(string input)
    {
        // Replace newline and tab characters with a space
        return input.Replace("\n", " ").Replace("\t", " ").Trim();
    }

    public List<Report> GetAllReports()
    {
        return reports;
    }

}
