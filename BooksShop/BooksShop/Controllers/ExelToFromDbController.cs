using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using System; 
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BooksShop.Controllers
{
    public class ExelToFromDbController : Controller
    {
        //Application _excelApp = new Application();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Load()
        {
            return RedirectToAction("Autors", "Autors");
        }

        public async Task<IActionResult> Download()
        {
            return RedirectToAction("Index");
        }
    }
}
