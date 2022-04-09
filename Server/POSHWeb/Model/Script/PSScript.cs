using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Management.Automation;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.Options;
using POSHWeb.Services;

namespace POSHWeb.Model
{
    public class PSScript : BaseEntity
    {
        public PSScript()
        {
            this.Parameters = new HashSet<PSParameter>();
        }

        [Key] public int Id { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public string? Description { get; set; }
        public string ContentHash { get; set; }
        public ICollection<PSParameter> Parameters { get; set; }
        public string Content { get; set; }
    }
}