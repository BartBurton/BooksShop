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
    public class ExelToFromDbController : Controller
    {
        private readonly booksshopContext _bs;
        public ExelToFromDbController(booksshopContext bs)
        {
            _bs = bs;
        }



        public IActionResult Excel()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Load()
        {
            if(Request.Form.Files.Count != 0)
            {
                var file = Request.Form.Files.First();
                ExcelToDbFile(file);
                return RedirectToAction("Autors", "Autors");
            }

            return RedirectToAction("Excel");
        }

        public async Task<FileResult> Download()
        {
            var books = await _bs.Books
                .Include(b => b.BooksAutors)
                    .ThenInclude(ba => ba.IdAutorNavigation)
                .ToListAsync();

            return DbToExcelFile(books);
        }



        private FileResult DbToExcelFile(List<Book> books)
        {
            var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Books Shop");

            sheet.Cell("A2").Value = "Название";    
            sheet.Cell("B2").Value = "Издание";    
            sheet.Cell("C2").Value = "Издатель";    
            sheet.Cell("D2").Value = "Описание";    
            sheet.Cell("E2").Value = "Автор";    
            sheet.Cell("F2").Value = "Дата рождения автора";
            sheet.Range("A2:F2").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
            sheet.Row(2).Style.Font.SetFontSize(18);
            sheet.Row(2).Style.Font.SetBold(true);

            long count = 3;
            foreach (var book in books)
            {
                sheet.Cell("A" + count).Value = book.Title;
                sheet.Cell("B" + count).Value = book.Edition;
                sheet.Cell("C" + count).Value = book.PublishedAt;
                sheet.Cell("D" + count).Value = book.Description;
                foreach (var autor in book.BooksAutors)
                {
                    sheet.Cell("E" + count).Value = autor.IdAutorNavigation.Name;
                    sheet.Cell("F" + count).Value = autor.IdAutorNavigation.Dob?.ToString("d");
                    count++;
                }
                count += (book.BooksAutors.Count == 0) ? 1 : 0;
            }

            sheet.Cell("A1").Value = count - 3;
            sheet.Range("A2:F" + (count - 1)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            sheet.Range("A2:F" + (count - 1)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.Range("A2:F2").Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            sheet.Range("A3:F" + count).Style.Fill.BackgroundColor = XLColor.FromArgb(216, 228, 188);
            sheet.Range("A3:F" + count).Style.Font.SetFontSize(14);
            sheet.Columns().AdjustToContents();

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Books Shop.xlsx");
            }
        }

        private void ExcelToDbFile(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var sheet = workbook.Worksheet(1);

            int count = Convert.ToInt32(sheet.Cell(1, 1).Value);
            var rows = sheet.RangeUsed().Rows(3, 2 + count);

            Book book = null;
            Autor autor;
            BooksAutor ba;
            foreach (var row in rows)
            {
                if(row.Cell(1).Value.ToString() != "")
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
    }
}
