using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    public class Images
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();
        
    }
}