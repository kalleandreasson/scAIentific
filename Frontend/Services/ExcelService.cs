using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using OfficeOpenXml;


namespace Frontend.Services
{
    public class ExcelService
    {
        public async Task<List<ResearchModel>> ReadExcelAsync(Stream fileStream)
        {
            var researches = new List<ResearchModel>();

            try
            {
                using (var package = new ExcelPackage())
                {
                    await package.LoadAsync(fileStream);
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        throw new Exception("No worksheet found in the Excel file.");
                    }

                    var rowCount = worksheet.Dimension?.Rows;
                    if (!rowCount.HasValue)
                    {
                        throw new Exception("The worksheet is empty.");
                    }

                    for (int row = 2; row <= rowCount.Value; row++)
                    {
                        if (!int.TryParse(worksheet.Cells[row, 4].Text, out int year))
                        {
                            // Handle the case where year is not a valid integer
                            continue; // or throw an exception or add to error list
                        }

                        var model = new ResearchModel
                        {
                            ArticleID = worksheet.Cells[row, 1].Text,
                            Authors = worksheet.Cells[row, 2].Text,
                            Title = worksheet.Cells[row, 3].Text,
                            Year = year,
                            Abstract = worksheet.Cells[row, 5].Text,
                            FullReference = worksheet.Cells[row, 6].Text,
                            Notes = worksheet.Cells[row, 7].Value?.ToString()
                        };

                        researches.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or add to an error list
                throw new Exception($"An error occurred while reading the Excel file: {ex.Message}");
            }

            return researches;
        }

    }
}