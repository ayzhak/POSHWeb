using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using POSHWeb.Data;
using POSHWeb.Model;
using POSHWebTests.Mocks;
using Xunit;

namespace POSHWeb.Services.Tests;

public class PSFileServiceTests
{
    [Fact]
    public void Create_CreateValidFile_Valid()
    {
        var fullPath = GeneratePSFile("Write-Host 'Test'");
        var psfileService = FactoryPSFileService(out var dbContext);
        psfileService.Create(fullPath);

        var scripts = dbContext.Script.ToList();
        Assert.Equal(scripts.Count, 1);
        var script = scripts[0];
        Assert.Equal(Path.GetFileName(fullPath), script.FileName);
        Assert.Equal(fullPath, script.FullPath);
        Assert.Equal("Write-Host 'Test'", script.Content);
        Assert.Equal("fe8aca7ccf892ab45f1956f5cacc628d98cb54aa601acc467aedd3f128a070d2", script.ContentHash);
        File.Delete(fullPath);
        dbContext.Dispose();
    }

    [Fact]
    public void Remove_ValidRemove_DatabaseShouldBeEmpty()
    {
        var psfileService = FactoryPSFileService(out var dbContext);
        var fullPath = GeneratePSFile("Write-Host 'Test'");

        dbContext.Script.Add(new PSScript
        {
            FullPath = fullPath,
            FileName = Path.GetFileName(fullPath),
            Content = "Write-Host 'Test'",
            ContentHash = "fe8aca7ccf892ab45f1956f5cacc628d98cb54aa601acc467aedd3f128a070d2"
        });

        dbContext.SaveChanges();
        File.Delete(fullPath);
        psfileService.Remove(fullPath);
        Assert.Empty(dbContext.Script.ToList());

        dbContext.Dispose();
    }

    [Fact]
    public void Modified_ContentChange_ContentInDatabaseChanged()
    {
        var psfileService = FactoryPSFileService(out var dbContext);
        var fullPath = GeneratePSFile("Write-Host 'Test'");
        dbContext.Script.Add(new PSScript
        {
            FullPath = fullPath,
            FileName = Path.GetFileName(fullPath),
            Content = "Write-Host 'Test'",
            ContentHash = "fe8aca7ccf892ab45f1956f5cacc628d98cb54aa601acc467aedd3f128a070d2",
            Parameters = new List<PSParameter>()
        });
        dbContext.SaveChanges();
        File.WriteAllText(fullPath, "Write-Host 'TEST'");
        psfileService.Modified(fullPath);

        var scripts = dbContext.Script.ToList();
        Assert.Equal(scripts.Count, 1);
        var script = scripts[0];
        Assert.Equal(Path.GetFileName(fullPath), script.FileName);
        Assert.Equal(fullPath, script.FullPath);
        Assert.Equal("Write-Host 'TEST'", script.Content);
        Assert.Equal("c1257f0f1f00a0f8621769785e7db7f5cedfb90649928c5cbedc957ecd670c59", script.ContentHash);

        File.Delete(fullPath);
        dbContext.Dispose();
    }

    [Fact]
    public void Rename_RenameFile_OnlyFilenameChangedInDatabase()
    {
        var psfileService = FactoryPSFileService(out var dbContext);
        var oldPathFile = GeneratePSFile("Write-Host 'Test'");
        var newPathFile = GeneratePSFile("Write-Host 'Test'");
        dbContext.Script.Add(new PSScript
        {
            FullPath = oldPathFile,
            FileName = Path.GetFileName(oldPathFile),
            Content = "Write-Host 'Test'",
            ContentHash = "fe8aca7ccf892ab45f1956f5cacc628d98cb54aa601acc467aedd3f128a070d2"
        });
        dbContext.SaveChanges();
        File.Delete(oldPathFile);
        psfileService.Rename(oldPathFile, newPathFile);

        var scripts = dbContext.Script.ToList();
        Assert.Equal(scripts.Count, 1);
        var script = scripts[0];
        Assert.Equal(newPathFile, script.FullPath);
        Assert.Equal(Path.GetFileName(newPathFile), script.FileName);
        Assert.Equal("Write-Host 'Test'", script.Content);
        Assert.Equal("fe8aca7ccf892ab45f1956f5cacc628d98cb54aa601acc467aedd3f128a070d2", script.ContentHash);

        File.Delete(newPathFile);
        dbContext.Dispose();
    }

    private string GeneratePSFile(string content)
    {
        var filePath = Path.GetTempPath() + Guid.NewGuid() + ".ps1";
        File.WriteAllText(filePath, content);
        return filePath;
    }

    private static PSFileService FactoryPSFileService(out DatabaseContext dbContext)
    {
        var servicesMock = MockDBContext.CreateMockIServiceScopeFactoryForDB(out var context);
        dbContext = context;

        var psfileService = new PSFileService(
            servicesMock.Object,
            new HasherService(),
            new NullLogger<PSFileService>(),
            new PSScriptValidator(new NullLogger<PSScriptValidator>()),
            new PSParameterParserService());
        return psfileService;
    }
}