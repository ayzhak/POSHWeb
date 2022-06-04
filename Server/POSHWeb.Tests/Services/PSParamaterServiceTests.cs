using System.Linq;
using System.Management.Automation.Language;
using POSHWeb.Enum;
using Xunit;

namespace POSHWeb.Services.Tests;

public class PSParamaterServiceTests
{
    private readonly PSParserService _psParameterParserService = new();

    [Fact]
    public void ParseParametersTest()
    {
        var script = @"
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$GivenName
)

Write-Host 'Test'
";
        Token[] tokens;
        ParseError[] parseErrors;
        var ast = Parser.ParseInput(script, out tokens, out parseErrors);
        var parametersAst = ast.FindAll(ast1 => ast1 is ParameterAst, false)
            .Cast<ParameterAst>()
            .ToList();

        Assert.Single(parametersAst);
        var parameter = parametersAst[0];

        var parsed = _psParameterParserService.Parse(parameter);
        Assert.Equal("GivenName", parsed.Name);
        Assert.True(parsed.Mandatory);
        Assert.Equal(SupportedType.String, parsed.Type);
    }

    [Fact]
    public void ParseParametersTest13()
    {
        var script = @"
param(
    [Parameter(Mandatory, Position=0)]
    [string]$GivenName
)

Write-Host 'Test'
";
        Token[] tokens;
        ParseError[] parseErrors;
        var ast = Parser.ParseInput(script, out tokens, out parseErrors);
        var parametersAst = ast.FindAll(ast1 => ast1 is ParameterAst, false)
            .Cast<ParameterAst>()
            .ToList();

        Assert.Single(parametersAst);
        var parameter = parametersAst[0];

        var parsed = _psParameterParserService.Parse(parameter);
        Assert.Equal("GivenName", parsed.Name);
        Assert.True(parsed.Mandatory);
        Assert.Equal(SupportedType.String, parsed.Type);
    }

    [Fact]
    public void ParseParametersTest2()
    {
        var script = @"
param(
    [string]$GivenName
)

Write-Host 'Test'
";
        Token[] tokens;
        ParseError[] parseErrors;
        var ast = Parser.ParseInput(script, out tokens, out parseErrors);
        var parametersAst = ast
            .FindAll(ast1 => ast1 is ParameterAst, false)
            .Cast<ParameterAst>().ToList();

        Assert.Single(parametersAst);
        var parameter = parametersAst[0];

        var parsed = _psParameterParserService.Parse(parameter);
        Assert.Equal("GivenName", parsed.Name);
        Assert.False(parsed.Mandatory);
        Assert.Equal(SupportedType.String, parsed.Type);
    }

    [Fact]
    public void ParseParametersTest3()
    {
        var script = @"
param(
    $GivenName
)

Write-Host 'Test'
";
        Token[] tokens;
        ParseError[] parseErrors;
        var ast = Parser.ParseInput(script, out tokens, out parseErrors);
        var parametersAst = ast
            .FindAll(ast1 => ast1 is ParameterAst, false)
            .Cast<ParameterAst>().ToList();

        Assert.Single(parametersAst);
        var parameter = parametersAst[0];

        var parsed = _psParameterParserService.Parse(parameter);
        Assert.Equal("GivenName", parsed.Name);
        Assert.False(parsed.Mandatory);
        Assert.Equal(SupportedType.None, parsed.Type);
    }

    [Fact]
    public void ParseParametersTest4()
    {
        var script = @"
param(
    $GivenName,
    $LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Equal(2, parameters.Count);

        var parameter1 = parameters[0];
        Assert.Equal("GivenName", parameter1.Name);
        Assert.False(parameter1.Mandatory);
        Assert.Equal(0, parameter1.Order);

        var parameter2 = parameters[1];
        Assert.Equal("LastName", parameter2.Name);
        Assert.False(parameter2.Mandatory);
        Assert.Equal(1, parameter2.Order);
    }

    [Fact]
    public void ParseParametersTest5()
    {
        var script = @"
param(
    $GivenName,
    [string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Equal(2, parameters.Count);

        var parameter1 = parameters[0];
        Assert.Equal("GivenName", parameter1.Name);
        Assert.False(parameter1.Mandatory);
        Assert.Equal(0, parameter1.Order);

        var parameter2 = parameters[1];
        Assert.Equal("LastName", parameter2.Name);
        Assert.False(parameter2.Mandatory);
        Assert.Equal(1, parameter2.Order);
    }

    [Fact]
    public void ParseParametersTest6()
    {
        var script = @"
param(
    [ValidateSet(""Steve"",""Mary"")]
    [string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal("Steve", parameter.Options.ValidValues?[0]);
        Assert.Equal("Mary", parameter.Options.ValidValues?[1]);
    }

    [Fact]
    public void ParseParametersTest7()
    {
        var script = @"
param(
    [ValidatePattern(""^DV-\d{7}"")]
[string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal("^DV-\\d{7}", parameter.Options.RegexString);
    }

    [Fact]
    public void ParseParametersTest8()
    {
        var script = @"
param(
    [ValidateLength(1, 10)]
[string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal(1, parameter.Options.MinLength);
        Assert.Equal(10, parameter.Options.MaxLength);
    }

    [Fact]
    public void ParseParametersTest9()
    {
        var script = @"
param(
    [ValidateRange(1, 10)]
[string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal(1, parameter.Options.MinValue);
        Assert.Equal(10, parameter.Options.MaxValue);
    }

    [Fact]
    public void ParseParametersTest10()
    {
        var script = @"
param(
    [ValidateRange(0.01, 0.1)]
[string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal(0.01, parameter.Options.MinValue);
        Assert.Equal(0.1, parameter.Options.MaxValue);
    }

    [Fact]
    public void ParseParametersTest11()
    {
        var script = @"
param(
    [ValidateScript({Test-Path -Path $_ -PathType Container})]
    [string]$Path
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("Path", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal("{Test-Path -Path $_ -PathType Container}", parameter.Options.ScriptBlock);
    }

    [Fact]
    public void ParseParametersTest12()
    {
        var script = @"
param(
    [ValidatePattern(""^DV-\d{7}"", ErrorMessage=""Test error message"")]
[string]$LastName
)

Write-Host 'Test'
";
        var parameters = _psParameterParserService.Parse(script).ToList();
        Assert.Single(parameters);

        var parameter = parameters[0];
        Assert.Equal("LastName", parameter.Name);
        Assert.False(parameter.Mandatory);
        Assert.Equal(0, parameter.Order);
        Assert.Equal("^DV-\\d{7}", parameter.Options.RegexString);
        Assert.Equal("Test error message", parameter.ErrorMessage);
    }
}