namespace POSHWeb.Exceptions;

public class ParameterParseException : Exception
{
    public ParameterParseException(string message)
        : base(message)
    {
    }
}