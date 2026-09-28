using System.Reflection;
using Core.Utils.LogUtils;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Configuration;
using Worker.Application.CustomModels;
using Worker.Application.CustomModels.Dtos;
using Worker.Application.CustomModels.Others;
using Worker.Application.Interface;
using Worker.Application.Services;
using Xunit;

namespace Worker.Application.Tests;

public sealed class CsvImportTests
{
    [Theory]
    [InlineData(3, true)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(4, false)]
    public async Task CsvIsDeletedOnlyAfterSuccessfulImport(int line, bool succeeds)
    {
        var folder = Path.Combine(Path.GetTempPath(), "server-service-csv-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            var file = Path.Combine(folder, $"LINE{line}_20260928.csv");
            var csv = line == 3 ? BuildCsv<EcuCsvDataLine3>() : BuildCsv<EcuCsvDataLine4>();
            await File.WriteAllTextAsync(file, csv);

            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FolderScanLine3"] = folder,
                ["FolderScanLine4"] = folder,
                ["FileNameLine3"] = "LINE3_",
                ["FileNameLine4"] = "LINE4_"
            }).Build();
            var client = new FakeClient(succeeds);
            var worker = new WorkerServiceImpl(config, new TestLogger(), client);

            if (line == 3)
                await worker.ScanFolderLine3();
            else
                await worker.ScanFolderLine4();

            Assert.Equal(!succeeds, File.Exists(file));
            Assert.Single(client.Imported);
            Assert.Equal("HU001", client.Imported[0].HUCode);
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    private static string BuildCsv<T>()
    {
        var columns = typeof(T).GetProperties()
            .Select(property => new
            {
                property.Name,
                Position = property.GetCustomAttribute<IndexAttribute>()?.Index ?? -1
            })
            .Where(column => column.Position >= 0)
            .OrderBy(column => column.Position)
            .ToArray();
        var values = columns.Select(column => column.Name switch
        {
            "DateManufacture" => "2026-09-28 10:00:00",
            "HUCode" => "HU001",
            "LaserPrinting" => "LASER001",
            "Result" => "OK",
            "NgCode" => "0000",
            _ => string.Empty
        });
        return string.Join(',', columns.Select(column => column.Name)) + Environment.NewLine
            + string.Join(',', values) + Environment.NewLine;
    }

    private sealed class FakeClient(bool succeeds) : IWorkerServiceClient
    {
        public List<EcuDataDto> Imported { get; } = [];

        public Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data)
        {
            Imported.AddRange(data);
            ServiceResult result = succeeds ? new ServiceResultSuccess() : new ServiceResultError("Import failed");
            return Task.FromResult(result);
        }
    }

    private sealed class TestLogger : ILoggerManager
    {
        public void LogInfo(string message) { }
        public void LogInfo(object data) { }
        public void LogDebug(object data) { }
        public void LogError(object data) { }
        public void LogWarning(object data) { }
        public void LogTrace(object data) { }
        public void LogDebug(string message) { }
        public void LogError(string message) { }
        public void LogError(Exception ex, string message) { }
        public void LogError(Exception ex) { }
        public void LogWarning(string message) { }
        public void LogTrace(string message) { }
    }
}
