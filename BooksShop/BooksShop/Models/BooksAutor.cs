using System;
using System.Collections.Generic;

#nullable disable

namespace BooksShop.Models
{
    public partial class BooksAutor
    {
        public long IdBook { get; set; }
        public long IdAutor { get; set; }

        public virtual Autor IdAutorNavigation { get; set; }
        public virtual Book IdBookNavigation { get; set; }
    }
}
