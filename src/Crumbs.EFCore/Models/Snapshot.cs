using System;
using System.ComponentModel.DataAnnotations;

namespace Crumbs.EFCore.Models
{
    public class Snapshot
    {
        [Key]
        public Guid AggregateId { get; set; }
        public int Version { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Content { get; set; }
    }
}