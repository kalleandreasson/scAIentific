using System.Text.RegularExpressions;
using datasettest.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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

    private void ParseDataset()
    {

        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(folderPath, false))
        {
            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
            SharedStringTablePart sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            SharedStringTable sharedStrings = sharedStringTablePart?.SharedStringTable;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            Dictionary<string, int> columnIndices = new Dictionary<string, int>
            {
                { "A", 0 },
                { "B", 1 },
                { "C", 2 },
                { "D", 3 },
                { "E", 4 },
                { "F", 5 },
            };

            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex != null && r.RowIndex.Value == 1) continue;

                Report report = new Report();
                string[] cellValues = new string[columnIndices.Count];

                foreach (Cell cell in r.Elements<Cell>())
                {
                    // Determine the column index of this cell
                    string columnReference = new String(cell.CellReference.Value.TakeWhile(char.IsLetter).ToArray());
                    if (columnIndices.TryGetValue(columnReference, out int columnIndex))
                    {
                        // Retrieve the cell value
                        cellValues[columnIndex] = GetCellValue(workbookPart, cell, sharedStrings);
                    }
                }

                // Assign values to report properties
                report.ArticleID = cellValues[columnIndices["A"]];
                report.Authors = cellValues[columnIndices["B"]];
                report.Title = cellValues[columnIndices["C"]];
                report.Year = int.TryParse(cellValues[columnIndices["D"]], out int year) ? year : 0;
                report.Abstract = cellValues[columnIndices["E"]];
                report.FullReference = cellValues[columnIndices["F"]];
                // ... Assign additional properties if necessary

                reports.Add(report);
            }
        }
    }

    private string GetColumnReference(string cellReference)
    {
        // Match the column reference (not the row number) and return it.
        return Regex.Match(cellReference, "[A-Za-z]+").Value;
    }

    private string GetCellValue(WorkbookPart workbookPart, Cell cell, SharedStringTable sharedStrings)
    {
        if (cell == null || cell.CellValue == null)
        {
            return null;
        }

        var value = cell.CellValue.InnerText;
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            return sharedStrings.ChildElements[int.Parse(value)].InnerText;
        }
        else
        {
            return value;
        }
    }

    public List<Report> GetAllReports()
    {
        return reports;
    }

}
