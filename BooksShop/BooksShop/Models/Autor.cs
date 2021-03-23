using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BooksShop.Models
{
    public partial class Autor
    {
        public Autor()
        {
            BooksAutors = new HashSet<BooksAutor>();
        }

        public long Id { get; set; }
        [Required(ErrorMessage = "Введите имя!")]
        public string Name { get; set; }
        public DateTime? Dob { get; set; }

        public virtual ICollection<BooksAutor> BooksAutors { get; set; }
    }
}
