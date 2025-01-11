using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Portfolios")] // This is the table name in the database // For the record add this if you are in a many to many relationship with a table.
    public class Portfolio
    {
        public string AppUserId { get; set; } // foreign key
        public int StockId { get; set; }  // foreign key
        public AppUser AppUser { get; set; } // navigation property
        public Stock Stock { get; set; } // navigation property
        // What is a navigation property?
        // A navigation property is a property that navigates from one entity type to another.
        // In this case, the AppUser property navigates from the AppUser entity to the Portfolio entity.
        // The Stock property navigates from the Stock entity to the Portfolio entity.
        // The AppUser property is a foreign key in the Portfolio entity.
        // a clear representation in real life is a person has a portfolio of stocks
    }
}