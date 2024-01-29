using Microsoft.AspNetCore.Mvc;
using datasettest.Services;
using datasettest.Models;

namespace datasettest.Controllers;

[ApiController]
[Route("api/dataset")]
public class DatasetController : ControllerBase
{

    private readonly Parser parser;

    public DatasetController()
    {
         parser = new Parser();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet(Name = nameof(TestDataset))]
    public async Task<ActionResult> TestDataset()
    {
        var reports = parser.GetAllReports();
        return Ok(reports);
    }

}