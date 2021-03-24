using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BooksShop.Models;

namespace BooksShop.Controllers
{
    public class BooksController : Controller
    {
        private readonly booksshopContext _bs;
        public BooksController(booksshopContext bs)
        {
            _bs = bs;
        }

        /// <summary>
        /// Страница списка всех книг.
        /// </summary>
        public async Task<IActionResult> Books()
        {
            var books = await _bs.Books.ToListAsync();

            return View(books);
        }
        


        #region Создание книги

        /// <summary>
        /// Страница создания книги.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Owners = new List<long>();
            ViewBag.Autors = await _bs.Autors
                .ToListAsync();

            return View();
        }

        /// <summary>
        /// Сохранение нового книги в базу.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Book model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Owners = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
                ViewBag.Autors = await _bs.Autors.ToListAsync();
                return View();
            }
            var Idautors = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
            model.BooksAutors = Idautors.Select(e => new BooksAutor() { IdAutor = e, IdBookNavigation = model }).ToList();

            await _bs.AddAsync(model);
            await _bs.SaveChangesAsync();

            return RedirectToAction("Books");
        }

        #endregion


        #region Информация о книге

        /// <summary>
        /// Страница информации о книге и списка ее авторов.
        /// </summary>
        /// <param name="id">ID книги</param>
        public async Task<IActionResult> Read(long? id)
        {
            Book book = await _bs.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) RedirectToAction("Books");


            _bs.Entry(book)
                .Collection(b => b.BooksAutors)
                .Query()
                .Include(ba => ba.IdAutorNavigation)
                .Load();

            return View(book);
        }

        #endregion


        #region Обновление информации о книге

        /// <summary>
        /// Страница обновления информации о книге.
        /// </summary>
        /// <param name="id">ID книги</param>
        [HttpGet]
        public async Task<IActionResult> Update(long? id, string source)
        {
            Book book = await _bs.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) RedirectToAction("Books");


            ViewBag.Owners = await _bs.BooksAutors
                .Where(ba => ba.IdBook == book.Id)
                .Select(ba => ba.IdAutor)
                .ToListAsync();
            ViewBag.Autors = await _bs.Autors
                .ToListAsync();

            ViewData["Source"] = source;
            return View(book);
        }

        /// <summary>
        /// Сохраннеие изменений информации о книге.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Update(Book model, string source)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Owners = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
                ViewBag.Autors = await _bs.Autors.ToListAsync();
                return View(); 
            }

            Book book = await _bs.Books.FirstOrDefaultAsync(a => a.Id == model.Id);
            _bs.Entry(book).Collection(b => b.BooksAutors).Load();
            var Idautors = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();


            if (book != null)
            {
                book.Title = model.Title;
                book.Description = model.Description;
                book.Edition = model.Edition;
                book.PublishedAt = model.PublishedAt;
                book.BooksAutors = Idautors.Select(e => new BooksAutor() { IdAutor = e, IdBook = book.Id }).ToList();

                await _bs.SaveChangesAsync();
            }

            return (source != null) ? Redirect(source) : RedirectToAction("Read", new { book.Id });
        }

        #endregion


        #region Удаление книги

        /// <summary>
        /// Удаление книги из базы.
        /// </summary>
        /// <param name="id">ID книги</param>
        public async Task<IActionResult> Delete(long? id, string source)
        {
            Book book = await _bs.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book != null)
            {
                _bs.Books.Remove(book);
                await _bs.SaveChangesAsync();
            }

            return (source != null) ? Redirect(source) : RedirectToAction("Books");
        }

        #endregion
    }
}
