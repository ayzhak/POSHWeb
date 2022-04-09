using System.Management.Automation.Language;
using System.Text.RegularExpressions;
using POSHWeb.Model;
using POSHWeb.Model.Script;

namespace POSHWeb.Services;

public class PSParameterParserService
{
    public ICollection<PSParameter> ExtractParameters(string script)
    {
        Token[] tokens;
        ParseError[] parseErrors;
        var ast = Parser.ParseInput(script, out tokens, out parseErrors);
        var parametersAst = ast.FindAll(ast1 => ast1 is ParameterAst, false).Cast<ParameterAst>().ToList();
        var parameters = new List<PSParameter>();
        var counter = 0;
        parametersAst.ForEach(parameterAst =>
        {
            var parameter = ConvertToPSParameter(parameterAst);
            parameter.Order = counter++;
            parameters.Add(parameter);
        });
        return parameters;
    }

    public PSParameter ConvertToPSParameter(ParameterAst ast)
    {
        return new PSParameter
        {
            Name = ParseName(ast),
            Mandatory = IsMandatory(ast),
            Type = ParseParameterType(ast),
            Options = ParseAttributes(ast),
            ErrorMessage = ExtractErrorMessage(ast),
            Default = ParseDefaultValue(),
            HelpMessage = ParseHelpMessage(ast)
        };
    }

    private string? ParseHelpMessage(ParameterAst ast)
    {
        var attributeAst = ast.FindAll(ast1 => ast1 is AttributeAst, false).Cast<AttributeAst>();
        foreach (var parameterAstAttribute in attributeAst)
        {
            if (!parameterAstAttribute.TypeName.ToString().Equals("Parameter")) continue;
            // Check of Named arguments
            foreach (var namedAttributeArgumentAst in parameterAstAttribute.NamedArguments)
                if (namedAttributeArgumentAst.ArgumentName.ToLower().Equals("helpmessage"))
                    return RemoveQuotes(namedAttributeArgumentAst.Argument.ToString());
        }

        return null;
    }

    private string ParseDefaultValue()
    {
        return "";
    }

    public string ParseName(ParameterAst parameter)
    {
        var name = parameter.Name.ToString();
        if (name[0] != '$') throw new ArgumentException();
        return name.Substring(1);
    }

    private bool IsMandatory(ParameterAst ast)
    {
        var attributeAst = ast.FindAll(ast1 => ast1 is AttributeAst, false).Cast<AttributeAst>();
        foreach (var parameterAstAttribute in attributeAst)
        {
            if (!parameterAstAttribute.TypeName.ToString().ToLower().Equals("parameter")) continue;
            // Check of Named arguments
            foreach (var namedAttributeArgumentAst in parameterAstAttribute.NamedArguments)
                if (namedAttributeArgumentAst.ArgumentName.ToLower().Equals("mandatory"))
                {
                    var value = namedAttributeArgumentAst.Argument.ToString().ToLower();
                    return value is "mandatory" or "$true";
                }
        }

        return false;
    }

    private bool FindAttribute(ParameterAst parameter, string AttributeName, out AttributeAst attribute)
    {
        attribute = null;
        var attributeAst = parameter
            .FindAll(ast1 => ast1 is AttributeAst, false)
            .Cast<AttributeAst>()
            .Where(ast => ast.TypeName.ToString().ToLower().Equals(AttributeName.ToLower()))
            .ToList();

        if (attributeAst.Count == 0) return false;
        if (attributeAst.Count > 1) return false;

        attribute = attributeAst[0];
        return true;
    }

    private string ParseParameterType(ParameterAst ast)
    {
        return ast.StaticType.Name;
    }

    private PSParameterOptions ParseAttributes(ParameterAst parameter)
    {
        var options = new PSParameterOptions();
        options.RegexString = ParseAttributeValidatePattern(parameter);
        options.ScriptBlock = ParseAttributeValidateScript(parameter);
        options.ValidValues = ParseAttributeValidateSet(parameter);
        int minLength;
        int maxLength;
        if (ParseAttributeValidateLength(parameter, out minLength, out maxLength))
        {
            options.MinLength = minLength;
            options.MaxLength = maxLength;
        }

        double minValue;
        double maxValue;
        if (ParseAttributeValidateRange(parameter, out minValue, out maxValue))
        {
            options.MinValue = minValue;
            options.MaxValue = maxValue;
        }

        int minCount;
        int maxCount;
        if (ParseAttributeValidateCount(parameter, out minCount, out maxCount))
        {
            options.MinCount = minCount;
            options.MaxCount = maxCount;
        }

        return options;
    }

    private bool ParseAttributeValidateLength(ParameterAst parameter, out int MinLength, out int MaxLength)
    {
        AttributeAst attribute;
        MinLength = 0;
        MaxLength = 0;
        if (!FindAttribute(parameter, "ValidateLength", out attribute)) return false;
        if (attribute.PositionalArguments.Count != 2) throw new ArgumentException("There must be two arguments");
        MinLength = (int) ((ConstantExpressionAst) attribute.PositionalArguments[0]).Value;
        MaxLength = (int) ((ConstantExpressionAst) attribute.PositionalArguments[1]).Value;
        return true;
    }

    private string? ParseAttributeValidatePattern(ParameterAst parameter)
    {
        AttributeAst attribute;
        if (!FindAttribute(parameter, "ValidatePattern", out attribute)) return null;
        if (attribute.PositionalArguments.Count == 2)
            throw new NotSupportedException("Two arguments are currently not supported");
        if (attribute.PositionalArguments.Count == 1) return RemoveQuotes(attribute.PositionalArguments[0].ToString());
        var argument = attribute.NamedArguments.First(ast => ast.ArgumentName.Equals("regexString"));
        return RemoveQuotes(argument.Argument.ToString());
    }

    private bool ParseAttributeValidateRange(ParameterAst parameter, out double minValue, out double maxValue)
    {
        AttributeAst attribute;
        minValue = 0;
        maxValue = 0;
        if (!FindAttribute(parameter, "ValidateRange", out attribute)) return false;
        if (attribute.PositionalArguments.Count != 2) throw new ArgumentException("There must be two arguments");
        minValue = Convert.ToDouble(((ConstantExpressionAst) attribute.PositionalArguments[0]).Value);
        maxValue = Convert.ToDouble(((ConstantExpressionAst) attribute.PositionalArguments[1]).Value);
        return true;
    }

    private bool ParseAttributeValidateCount(ParameterAst parameter, out int minCount, out int maxCount)
    {
        AttributeAst attribute;
        minCount = 0;
        maxCount = 0;
        if (!FindAttribute(parameter, "ValidateCount", out attribute)) return false;
        if (attribute.PositionalArguments.Count != 2) throw new ArgumentException("There must be two arguments");
        minCount = (int) ((ConstantExpressionAst) attribute.PositionalArguments[0]).Value;
        maxCount = (int) ((ConstantExpressionAst) attribute.PositionalArguments[1]).Value;
        return true;
    }

    private string ParseAttributeValidateScript(ParameterAst parameter)
    {
        AttributeAst attribute;
        if (!FindAttribute(parameter, "ValidateScript", out attribute)) return null;
        if (attribute.PositionalArguments.Count == 1) return attribute.PositionalArguments[0].ToString();
        var argument = attribute.NamedArguments.First(ast => ast.ArgumentName.Equals("scriptBlock"));
        return RemoveQuotes(argument.Argument.ToString());
    }

    private string[] ParseAttributeValidateSet(ParameterAst parameter)
    {
        AttributeAst attribute;
        if (!FindAttribute(parameter, "ValidateSet", out attribute)) return null;
        var set = new List<string>();
        attribute.PositionalArguments.ToList().ForEach(ast => set.Add(RemoveQuotes(ast.ToString())));
        return set.ToArray();
    }

    private string? ExtractErrorMessage(ParameterAst parameter)
    {
        var attributesWithPossibleErrorMessage = new List<string> {"ValidateSet", "ValidateScript", "ValidatePattern"};
        var attributeAst = parameter
            .FindAll(ast1 => ast1 is AttributeAst, false)
            .Cast<AttributeAst>()
            .Where(ast => attributesWithPossibleErrorMessage.Contains(ast.TypeName.ToString()))
            .ToList();
        if (attributeAst.Count == 0) return null;
        if (attributeAst.Count > 1) throw new Exception("Has more than one parameter.");
        var attribute = attributeAst[0];
        var namedParameters = attribute.NamedArguments?
            .Where(ast => ast.ArgumentName.ToString().Equals("ErrorMessage"))
            .ToList();
        if (namedParameters.Count == 0) return null;
        if (namedParameters.Count > 1) throw new Exception("Has more than one parameter named parameter.");
        return RemoveQuotes(namedParameters[0].Argument.ToString());
    }

    private string RemoveQuotes(string text)
    {
        return Regex.Replace(text, "^[\"|']|[\"|']$", "");
    }
}