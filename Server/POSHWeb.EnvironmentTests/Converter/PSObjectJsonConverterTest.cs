using System.Management.Automation;
using Newtonsoft.Json;
using Xunit;

namespace POSHWeb.Environment.Util;

public class PSObjectJsonConverterTest
{
    public class Test
    {
        public string Name { get; set; } = "Name";
        public int Age { get; set; } = 50;
    }
    
    [Fact]
    public void TestToJson_string()
    {
        var pso = new PSObject("string");
        var json = PSObjectJsonConverter.ToJson(pso);
        Assert.Same("string", json);
    }
    
    [Fact]
    public void TestToJson()
    {
        var pso = new PSObject(new Test());
        var json = PSObjectJsonConverter.ToJson(pso);
        Assert.Same("string", json);
    }

    [Fact]
    public void Test_primtiiveconvert()
    {
        var pso = new PSObject(new Test());
        var json = JsonConvert.SerializeObject(pso.BaseObject);
        Assert.Same("", json);
    }
}