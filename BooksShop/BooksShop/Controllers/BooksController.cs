using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BooksShop.Models;

namespace BooksShop.Controllers
{
    /// <summary>
    /// Конроллер работы с данными о книгах
    /// </summary>
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
            //Т.к. книга только создается, у нее нет автора, поэтому Owners пустой
            ViewBag.Owners = new List<long>();
            //Для выбора авторов подгружаем всех аторов
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
                //Если модель не валидна востанавливаем список выбранных авторов
                ViewBag.Owners = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
                //Для выбора авторов подгружаем всех аторов
                ViewBag.Autors = await _bs.Autors.ToListAsync();
                return View();
            }
            //Получаем ID выбранных авторов
            var Idautors = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
            //Связываем книгу с указанными авторами
            model.BooksAutors = Idautors.Select(e => new BooksAutor() { IdAutor = e, IdBookNavigation = model }).ToList();

            await _bs.AddAsync(model);
            await _bs.SaveChangesAsync();

            return RedirectToAction("Read", new { model.Id });
        }

        #endregion


        #region Информация о книге

        /// <summary>
        /// Страница информации о книге и списка ее авторов.
        /// </summary>
        /// <param name="id">ID книги</param>
        public async Task<IActionResult> Read(long? id)
        {
            //Находим книгу
            Book book = await _bs.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) RedirectToAction("Books");

            //Подгружаем ее авторов
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
            //Находим книгу
            Book book = await _bs.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) RedirectToAction("Books");

            //Загружаем индексы авторов книги
            ViewBag.Owners = await _bs.BooksAutors
                .Where(ba => ba.IdBook == book.Id)
                .Select(ba => ba.IdAutor)
                .ToListAsync();
            //Загружаем всех авторов для выбора
            ViewBag.Autors = await _bs.Autors
                .ToListAsync();

            //Сохраняем источник запроса для использования в Post методе
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
                //Если модель не валидна востанавливаем список выбранных авторов
                ViewBag.Owners = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
                //Для выбора авторов подгружаем всех аторов
                ViewBag.Autors = await _bs.Autors.ToListAsync();
                return View(); 
            }

            //Находим книгу и подгружаем связанные с ней данные из связующей таблицы
            Book book = await _bs.Books.FirstOrDefaultAsync(a => a.Id == model.Id);
            _bs.Entry(book).Collection(b => b.BooksAutors).Load();

            //Изменяем книгу если она есть
            if (book != null)
            {
                book.Title = model.Title;
                book.Description = model.Description;
                book.Edition = model.Edition;
                book.PublishedAt = model.PublishedAt;

                //Получаем ID выбранных авторов
                var Idautors = Request.Form["Autors"].Select(e => Convert.ToInt64(e)).ToList();
                //Связываем книгу с указанными авторами
                book.BooksAutors = Idautors.Select(e => new BooksAutor() { IdAutor = e, IdBook = book.Id }).ToList();

                await _bs.SaveChangesAsync();
            }

            //Если источник задан возвращаемся к нему, иначе на страницу книги
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

            //Если источник задан возвращаемся к нему, иначе на страницу книг
            return (source != null) ? Redirect(source) : RedirectToAction("Books");
        }

        #endregion
    }
}
