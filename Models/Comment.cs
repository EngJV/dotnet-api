using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")] // This is the table name in the database // For the record add this if you are in a many to many relationship with a table.
    public class Comment
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        // KEY
        public int? StockId { get; set; }
        // Navigation
        public Stock? Stock { get; set; }
    }
}