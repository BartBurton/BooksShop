using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BooksShop.Models;
using ClosedXML.Excel;

namespace BooksShop.Controllers
{
    /// <summary>
    /// Контроллер работы с Excel файлом.
    /// </summary>
    public class ExelToFromDbController : Controller
    {
        private readonly booksshopContext _bs;
        public ExelToFromDbController(booksshopContext bs)
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
            //Если файл загружен
            if(Request.Form.Files.Count != 0)
            {
                var file = Request.Form.Files.First();
                //Обноляет БД
                ExcelFileToDb(file);
                return RedirectToAction("Autors", "Autors");
            }

            return RedirectToAction("Excel");
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

            //Возвращаем файл excel
            return DbToExcelFile(books);
        }



        #region Работа с excel файлом

        /// <summary>
        /// Загрузить данные из БД в excel файл.
        /// </summary>
        /// <param name="books">Список книг, с всеми связанными сущностями</param>
        /// <returns>Файл для скачивания.</returns>
        private FileResult DbToExcelFile(List<Book> books)
        {
            //Создаем рабочую область
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Books Shop");

            //Устанавливаем загаловки таблицы
            sheet.Cell("A2").Value = "Название";
            sheet.Cell("B2").Value = "Издание";
            sheet.Cell("C2").Value = "Издатель";
            sheet.Cell("D2").Value = "Описание";
            sheet.Cell("E2").Value = "Автор";
            sheet.Cell("F2").Value = "Дата рождения автора";
            sheet.Range("A2:F2").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
            sheet.Row(2).Style.Font.SetFontSize(18);
            sheet.Row(2).Style.Font.SetBold(true);

            //Начинаем записывать данные с 3 строки, т.к. в первой
            //кол-во строк с данными, во второй заголовки
            long count = 3;
            foreach (var book in books)
            {
                sheet.Cell("A" + count).Value = book.Title;
                sheet.Cell("B" + count).Value = book.Edition;
                sheet.Cell("C" + count).Value = book.PublishedAt;
                sheet.Cell("D" + count).Value = book.Description;
                foreach (var autor in book.BooksAutors)
                {
                    //Данные об авторах книги записываются без дублирования данных
                    //о книге, но на новой строки, поэтому count растет
                    sheet.Cell("E" + count).Value = autor.IdAutorNavigation.Name;
                    sheet.Cell("F" + count).Value = autor.IdAutorNavigation.Dob?.ToString("d");
                    count++;
                }
                //Если у книги нет авторов count не инткрементировался в цикле
                count += (book.BooksAutors.Count == 0) ? 1 : 0;
            }

            //Кол-во строк с данными
            sheet.Cell("A1").Value = count - 3;
            //Стиль содержания таблицы
            sheet.Range("A2:F" + (count - 1)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            sheet.Range("A2:F" + (count - 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Range("A2:F2").Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            sheet.Range("A3:F" + count).Style.Fill.BackgroundColor = XLColor.FromArgb(216, 228, 188);
            sheet.Range("A3:F" + count).Style.Font.SetFontSize(14);
            sheet.Columns().AdjustToContents();

            //Возврат файла на скачивание
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Books Shop.xlsx");
            }
        }

        /// <summary>
        /// Загрузить данные из файла в базу данных
        /// </summary>
        /// <param name="file">Файл полученный из формы</param>
        private void ExcelFileToDb(IFormFile file)
        {
            //Считываем поток файла, открываем первую страницу
            var workbook = new XLWorkbook(file.OpenReadStream());
            var sheet = workbook.Worksheet(1);

            //В ячейке A1 указано кол-во строк с данными
            int count = Convert.ToInt32(sheet.Cell(1, 1).Value);
            //Считывание с 3 строки, т.к. во 2 заголовки таблицы
            var rows = sheet.RangeUsed().Rows(3, 2 + count);

            Book book = null;
            Autor autor;
            BooksAutor ba;
            foreach (var row in rows)
            {
                //Читаем данные для книги если, есть название
                //Если авторов книги несколько, экземпляр создастся один раз
                if (row.Cell(1).Value.ToString() != "")
                {
                    book = new Book()
                    {
                        Title = row.Cell(1).Value.ToString(),
                        Edition = row.Cell(2).Value.ToString(),
                        PublishedAt = row.Cell(3).Value.ToString(),
                        Description = row.Cell(4).Value.ToString()
                    };
                    _bs.Books.Add(book);
                }
                //Авторы связываются с текущим экземпляром book
                autor = new Autor()
                {
                    Name = row.Cell(5).Value.ToString(),
                    Dob = row.Cell(6).Value as DateTime?
                };
                ba = new BooksAutor()
                {
                    IdAutorNavigation = autor,
                    IdBookNavigation = book
                };

                _bs.Autors.Add(autor);
                _bs.BooksAutors.Add(ba);
                _bs.SaveChanges();
            }
        } 

        #endregion
    }
}
