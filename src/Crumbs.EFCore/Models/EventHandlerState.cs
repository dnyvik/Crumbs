using Crumbs.Core.Event.EventualConsistency;
using System;
using System.ComponentModel.DataAnnotations;

namespace Crumbs.EFCore.Models
{
    public class EventHandlerState
    {
        [Key]
        public Guid Id { get; set; }
        public long ProcessedEventId { get; set; } //Long?
        public DateTimeOffset LastUpdated { get; set; }
        public StatefulHandlerStatus Status { get; set; }

        [Required]
        public string Data { get; set; }
    }
}