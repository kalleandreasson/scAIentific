using ChatGPTAPI.Models;
using ClosedXML.Excel;
using System.Collections.Generic;
using ChatGPTAPI.Models;
using System.Text;

namespace ChatGPTAPI.Services;

public class Parser
{

    private readonly string folderPath;
    private readonly List<Report> reports;

    public Parser()
    {
        reports = new List<Report>();
    }

    public List<Report> ParseDataset(string folderPath)
    {
        Console.WriteLine("Inside parser");
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
        SaveReportsAsText(reports, folderPath);
        return reports;
    }

    public void SaveReportsAsText(List<Report> reports, string folderPath)
    {
        // Extract the directory from the folderPath
        var directory = Path.GetDirectoryName(folderPath);
        Console.WriteLine("Directory: " + directory);
        var txtFilePath = Path.Combine(directory, "ReportsSummary.txt");
        Console.WriteLine("txtFilePath: " + txtFilePath);

        var sb = new StringBuilder();
        foreach (var report in reports)
        {
            // Format the report data as you like
            sb.AppendLine($"ArticleID: {report.ArticleID}");
            sb.AppendLine($"Authors: {report.Authors}");
            sb.AppendLine($"Title: {report.Title}");
            sb.AppendLine($"Year: {report.Year}");
            sb.AppendLine($"Abstract: {report.Abstract}");
            sb.AppendLine($"FullReference: {report.FullReference}");
            sb.AppendLine("-------------------------------------------");
        }

        // Write the formatted text to a .txt file
        File.WriteAllText(txtFilePath, sb.ToString());
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