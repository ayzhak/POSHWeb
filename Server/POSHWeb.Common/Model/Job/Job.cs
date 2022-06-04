using System.Text.Json;
using System.Text.Json.Serialization;
using POSHWeb.Common.Model.Job;
using POSHWeb.Common.Model.Script;
using POSHWeb.Enum;
using POSHWeb.Environment.Model;
using POSHWeb.Model.Job.Logs;
using POSHWeb.Scheduler.Trigger;

namespace POSHWeb.Model.Job;

public class Job : BaseEntity
{
    public Job()
    {
        State = JobState.Created;
        Parameters = new List<InputParameter>();
        Runs = new List<JobRuns>();
        Triggers = new List<ScheduleTrigger>();
    }

    public int Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JobState State { get; set; }
    public PSScript Script { get; set; }
    public ICollection<InputParameter> Parameters { get; set; }
    public ICollection<ScheduleTrigger> Triggers { get; set; }
    public ICollection<JobRuns> Runs { get; set; }
    public RunspaceSettings RunspaceSettings { get; set; }
    public Credentials Credentials { get; set; }

    public void SetParameter(JsonElement element, PSScript script)
    {
        foreach (var property in element.EnumerateObject())
        {
            var scriptParameter = script.Parameters.First(parameter => parameter.Name == property.Name);
            var jobParameter = new InputParameter
            {
                Name = property.Name,
                Type = scriptParameter.Type
            };
            jobParameter.SetValue(property.Value);
            Parameters.Add(jobParameter);
        }
    }

    public Dictionary<string, object> GetParameters()
    {
        var list = new Dictionary<string, object>();
        foreach (var parameter in Parameters)
        {
            list.Add(parameter.Name, parameter.GetValue());
        }

        return list;
    }
}