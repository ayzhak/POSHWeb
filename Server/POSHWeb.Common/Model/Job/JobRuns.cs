using POSHWeb.Model;
using POSHWeb.Model.Job.Logs;

namespace POSHWeb.Common.Model.Script;

public class JobRuns: BaseEntity
{
    public JobRuns()
    {
        Log = new List<Log>();
    }
    public ICollection<Log> Log { get; set; }
}