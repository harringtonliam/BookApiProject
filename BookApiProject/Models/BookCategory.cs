using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookApiProject.Models
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public virtual Book Book {get; set;}

        public int CategpryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
