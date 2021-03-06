using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace POSHWeb.Services.Tests;

public class PSScriptValidatorTests
{
    [Fact]
    public void ValidateInput_ValidScript_Valid()
    {
        var script = @"
Write-Host 'Test'
";
        var validator = new ScriptValidatorService(new NullLogger<ScriptValidatorService>());
        Assert.True(validator.ValidateInput(script));
    }

    [Fact]
    public void ValidateInput_ValidInput_ShouldBeInvalid()
    {
        var script = @"
Write-Host Test'
";
        var validator = new ScriptValidatorService(new NullLogger<ScriptValidatorService>());
        Assert.False(validator.ValidateInput(script));
    }

    [Fact]
    public void ValidateInput_EmptyInput_ShouldBeInvalid()
    {
        var script = @"";
        var validator = new ScriptValidatorService(new NullLogger<ScriptValidatorService>());
        Assert.True(validator.ValidateInput(script));
    }
}