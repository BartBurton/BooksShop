using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BooksShop.Models;

namespace BooksShop.Controllers
{
    public class AutorsController : Controller
    {
        private readonly booksshopContext _bs;
        public AutorsController(booksshopContext bs)
        {
            _bs = bs;
        }

        /// <summary>
        /// Страница списка всех авторов.
        /// </summary>
        public async Task<IActionResult> Autors()
        {
            var autors = await _bs.Autors.ToListAsync();

            return View(autors);
        }



        #region Создание автора

        /// <summary>
        /// Страница создания автора.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Сохранение нового автора в базу.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Autor model)
        {
            if (!ModelState.IsValid) { return View(); }

            await _bs.AddAsync(model);
            await _bs.SaveChangesAsync();

            return RedirectToAction("Autors");
        }

        #endregion


        #region Информация об авторе

        /// <summary>
        /// Страница информации об авторе и списка написанных им книг
        /// </summary>
        /// <param name="id">ID автора</param>
        public async Task<IActionResult> Read(long? id)
        {
            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);
            if (autor == null) RedirectToAction("Autors");


            _bs.Entry(autor)
                .Collection(a => a.BooksAutors)
                .Query()
                .Include(ba => ba.IdBookNavigation)
                .Load();

            return View(autor);
        }

        #endregion


        #region Обновление информации об авторе

        /// <summary>
        /// Страница обновления информации об авторе.
        /// </summary>
        /// <param name="id">ID автора</param>
        [HttpGet]
        public async Task<IActionResult> Update(long? id, string source)
        {
            var x = Request.HttpContext.TraceIdentifier;

            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);
            if (autor == null) RedirectToAction("Autors");

            ViewData["Source"] = source;
            return View(autor);
        }

        /// <summary>
        /// Сохраннеие изменений информации об авторе.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Update(Autor model, string source)
        {
            if (!ModelState.IsValid) { return View(); }

            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == model.Id);

            if (autor != null)
            {
                autor.Name = model.Name;
                autor.Dob = model.Dob;
                await _bs.SaveChangesAsync();
            }

            return (source != null) ? Redirect(source) : RedirectToAction("Read", new { autor.Id });
        }

        #endregion


        #region Удаление автора

        /// <summary>
        /// Удаление автора из базы
        /// </summary>
        /// <param name="id">ID автора</param>
        public async Task<IActionResult> Delete(long? id, string source)
        {
            var x = Request.Query;
            
            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);

            if (autor != null)
            {
                _bs.Autors.Remove(autor);
                await _bs.SaveChangesAsync();
            }

            return (source != null) ? Redirect(source) : RedirectToAction("Autors");
        } 

        #endregion
    }
}
