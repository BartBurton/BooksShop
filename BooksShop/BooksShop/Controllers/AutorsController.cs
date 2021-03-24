using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BooksShop.Models;

namespace BooksShop.Controllers
{
    /// <summary>
    /// Контроллер работы с данными о авторах
    /// </summary>
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

            return RedirectToAction("Read", new { model.Id });
        }

        #endregion


        #region Информация об авторе

        /// <summary>
        /// Страница информации об авторе и списка написанных им книг
        /// </summary>
        /// <param name="id">ID автора</param>
        public async Task<IActionResult> Read(long? id)
        {
            //Находим автора
            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);
            if (autor == null) RedirectToAction("Autors");
            //Подгружаем данные о его книгах
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
            //Находим автора возврацаем форму
            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);
            if (autor == null) RedirectToAction("Autors");

            //Повторно сохраняем источник запроса, чтобы вернуться к нему из Post метода
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

            //Если источник задан возвращаемся к нему, иначе на страница автора
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
            Autor autor = await _bs.Autors.FirstOrDefaultAsync(a => a.Id == id);
            if (autor != null)
            {
                _bs.Autors.Remove(autor);
                await _bs.SaveChangesAsync();
            }

            //Если источник задан, возвращаемся к нему, иначе к списку авторов
            return (source != null) ? Redirect(source) : RedirectToAction("Autors");
        } 

        #endregion
    }
}
