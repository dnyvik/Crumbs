using System;
using System.ComponentModel.DataAnnotations;

namespace Crumbs.EFCore.Models
{
    public class Event
    {
        [Key]
        public long EventId { get; set; }
        public Guid AggregateId { get; set; }
        public Guid AppliedByUserId { get; set; }
        public Guid SessionId { get; set; }
        public int AggregateVersion { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Data { get; set; }
    }
}