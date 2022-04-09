using System.Text.Json.Serialization;
using POSHWeb.Enum;

namespace POSHWeb.Model;

public class Job : BaseEntity
{
    public Job()
    {
        State = JobState.Created;
    }

    public int Id { get; set; }
    public string FileName { get; set; }
    public string FullPath { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JobState State { get; set; }

    public string Content { get; set; }
    public string ContentHash { get; set; }
    public ICollection<InputParameter> Parameters { get; set; }
    public string Log { get; set; }
}