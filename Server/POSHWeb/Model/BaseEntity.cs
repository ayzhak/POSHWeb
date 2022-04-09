using System.ComponentModel;

namespace POSHWeb.Model;

public abstract class BaseEntity
{
    [ReadOnly(true)] public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}