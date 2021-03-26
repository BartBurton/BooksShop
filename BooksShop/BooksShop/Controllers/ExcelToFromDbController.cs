using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BooksShop.Models;
using Excel;
using ClosedXML.Excel;

namespace BooksShop.Controllers
{
    /// <summary>
    /// Контроллер работы с Excel файлом.
    /// </summary>
    public class ExcelToFromDbController : Controller
    {
        private readonly booksshopContext _bs;
        public ExcelToFromDbController(booksshopContext bs)
        {
            _bs = bs;
        }


        /// <summary>
        /// Страница работы с Excel файлом.
        /// </summary>
        public IActionResult Excel()
        {
            return View();
        }

        /// <summary>
        /// Производит загрузку данных из excel файла в БД
        /// </summary>
        [HttpPost]
        public IActionResult Load()
        {
            //Если файл не загружен
            if (Request.Form.Files.Count == 0) return RedirectToAction("Excel");
            
            //Обноляем БД
            var file = Request.Form.Files.First();
            List<object[]> body = ExcelFileInteraction.FromExcel(file.OpenReadStream());

            Book book = null;
            Autor autor = null;
            BooksAutor ba;
            foreach (var row in body)
            {
                //Читаем данные для книги если, есть название
                //Если авторов книги несколько, экземпляр создастся один раз
                if (row[0].ToString() != "")
                {
                    book = new Book()
                    {
                        Title = row[0].ToString(), Edition = row[1].ToString(),
                        PublishedAt = row[2].ToString(), Description = row[3].ToString()
                    };
                    _bs.Books.Add(book);
                }

                //Авторы связываются с текущим экземпляром book
                //Если у книги нет автора или строка пустая автор не создается
                if (row[4].ToString() != "")
                {
                    autor = new Autor() { Name = row[4].ToString(), Dob = row[5] as DateTime? };
                    _bs.Autors.Add(autor);
                }

                //Связывание проиходит только, если оба были загружены
                if (autor != null && book != null)
                {
                    ba = new BooksAutor() { IdAutorNavigation = autor, IdBookNavigation = book };
                    _bs.BooksAutors.Add(ba);
                }

                _bs.SaveChanges();
                //Если книга без автора нужно предотвратить с предыдущим автором.
                autor = null;
            }

            return RedirectToAction("Autors", "Autors");
        }

        /// <summary>
        /// Позваляет получить скачать данные БД в виде excel файла
        /// </summary>
        public async Task<FileResult> Download()
        {
            //Собираем необходимые данные из БД
            var books = await _bs.Books
                .Include(b => b.BooksAutors)
                    .ThenInclude(ba => ba.IdAutorNavigation)
                .ToListAsync();

            List<object[]> body = new List<object[]>();
            body.Add(new object[6]);
            foreach (var book in books)
            {
                body.Last()[0] = book.Title;
                body.Last()[1] = book.Edition;
                body.Last()[2] = book.PublishedAt;
                body.Last()[3] = book.Description;
                foreach (var ba in book.BooksAutors)
                { 
                    body.Last()[4] = ba.IdAutorNavigation.Name;
                    body.Last()[5] = ba.IdAutorNavigation.Dob;
                    body.Add(new object[6]);
                }
                if(book.BooksAutors.Count == 0) body.Add(new object[6]);
            }
            body.RemoveAt(body.Count - 1);

            string[] headers = new string[6] 
            {
                "Название", "Издание", "Издатель", "Описание", "Автор", "Дата рождения автора"
            };

            //Возвращаем файл excel
            return File(
                ((MemoryStream)ExcelFileInteraction.ToExcel("Books Shop", headers, body, 
                new StyleSheet() { HeaderBackgroundColor = (230, 184, 183), BodyBackgroundColor = (216, 228, 188) }))
                .ToArray(), 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Books Shop.xlsx");
        }
    }
}
