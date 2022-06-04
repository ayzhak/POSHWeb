
using POSHWeb.Common.Model.Script;

namespace POSHWeb.Model;

public class PSScript : BaseEntity
{
    public PSScript()
    {
        Parameters = new HashSet<PSParameter>();
    }

    public int Id { get; set; }
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public string Content { get; set; }
    public string ContentHash { get; set; }
    public string NewContent { get; set; }
    public string NewContentHash { get; set; }
    public PSHelp? Help { get; set; }
    public ICollection<PSParameter> Parameters { get; set; }
    public ICollection<Job.Job>? Jobs { get; set; }
    public ICollection<JobRuns>? Runs { get; set; }
}