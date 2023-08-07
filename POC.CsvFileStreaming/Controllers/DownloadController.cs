using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using POC.CsvFileStreaming.Services;

namespace POC.CsvFileStreaming.Controllers;

[ApiController]
[Route("Download")]
public class DownloadController : ControllerBase, IDisposable
{
    private readonly FileContentService _fileContentService;

    public DownloadController(IConfiguration config)
    {
        // Bad - use DI in the real world :) This is just a POC
        _fileContentService = new FileContentService(config);
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var textWriter = new StreamWriter(new MemoryStream());
        var csvWriter = new CsvWriter(textWriter, CultureInfo.CurrentCulture);
        
        // write header
        csvWriter.WriteHeader<Model>();

        var rows = _fileContentService.GetData();
        
        await csvWriter.WriteRecordsAsync(rows);
        await textWriter.FlushAsync();
        
        //reset to beginning of stream
        textWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        
        Console.WriteLine($"Returning file stream at {DateTime.Now:hh:mm:ss.fff}");
        return new FileStreamResult(textWriter.BaseStream, "text/csv")
        {
            FileDownloadName = "data.csv"
        };
    }

    public void Dispose()
    {
        Console.WriteLine($"disposing controller at {DateTime.Now:hh:mm:ss.fff}");
        _fileContentService.Dispose();
    }
}