using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lexiconn.Models;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // REFACTOR: split into several methods, this class surely lacks ones
        public async Task<IActionResult> Index()
        {
            var db = new DBDictionaryContext();
            var modelList = new List<WordData>();
            var catWords = await db.CategorizedWords.ToListAsync();
            
            foreach (var catWord in catWords)
            {
                var model = new WordData();
                var word = await db.Words.FirstOrDefaultAsync(w => w.Id == catWord.WordId);
                model.Word = word.ThisWord;
                
                var language = await db.Languages.FirstOrDefaultAsync(l => l.Id == word.LanguageId);
                model.Language = language.Name;
                model.LanguageId = word.LanguageId;

                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == catWord.CategoryId);
                model.Category = catWord.Category.Name;
                model.CategoryId = catWord.CategoryId;

                var translations = await db.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToListAsync();
                var translationIds = new List<int>();

                foreach (var translation in translations)
                {
                    translationIds.Add(translation.Id);
                }

                model.TranslationIds = string.Join(",", translationIds);

                string commaTranslations = "";

                for (int i = 0; i < translations.Count - 1; i++)
                {
                    commaTranslations += translations[i].ThisTranslation + ", ";
                }

                if (translations.Count != 0)
                {
                    commaTranslations += translations[translations.Count - 1].ThisTranslation;
                }
                model.Translation = commaTranslations;

                modelList.Add(model);
            }

            return View(modelList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
