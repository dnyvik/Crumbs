using System;
using System.ComponentModel.DataAnnotations;

namespace Crumbs.EFCore.Models
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset CompletedDate { get; set; }
        public Guid ComittedByUserId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
