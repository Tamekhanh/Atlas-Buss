using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        
    }
}