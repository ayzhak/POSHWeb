using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.FileIO;

namespace POSHWeb.Model;

public class SimpleInputParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
}