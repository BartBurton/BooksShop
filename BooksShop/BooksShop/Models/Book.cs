using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BooksShop.Models
{
    public partial class Book
    {
        public Book()
        {
            BooksAutors = new HashSet<BooksAutor>();
        }

        public long Id { get; set; }
        [Required(ErrorMessage = "Введите название!")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Edition { get; set; }
        public string PublishedAt { get; set; }

        public virtual ICollection<BooksAutor> BooksAutors { get; set; }
    }
}
