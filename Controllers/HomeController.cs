using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lexiconn.Models;

namespace Lexiconn.Controllers
{
    public class HomeController : Controller
    { 
        private readonly DBDictionaryContext _context;

        /// <summary>
        /// Creates the Home Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public HomeController(DBDictionaryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the view with all the word records to display on the home page.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var modelList = new List<WordData>();
            var catWords = await _context.CategorizedWords.ToListAsync();
            
            foreach (var catWord in catWords)
            {
                var model = new WordData();
                FillModel(model, catWord);
                modelList.Add(model);
            }

            return View(modelList);
        }

        /// <summary>
        /// Fills the word record model with all necessary information to store and display.
        /// </summary>
        /// <param name="model">The word record model.</param>
        /// <param name="catWord">Current categorized word in the foreach loop.</param>
        private void FillModel(WordData model, CategorizedWord catWord)
        {
            var word = _context.Words.FirstOrDefault(w => w.Id == catWord.WordId);
            var language = _context.Languages.FirstOrDefault(l => l.Id == word.LanguageId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == catWord.CategoryId);
            var translations = _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToList();
            var translationIds = new List<int>();

            foreach (var translation in translations)
            {
                translationIds.Add(translation.Id);
            }

            model.Word = word.ThisWord;
            model.WordId = catWord.WordId;
            model.Language = language.Name;
            model.LanguageId = word.LanguageId;
            model.Category = catWord.Category.Name;
            model.CategoryId = catWord.CategoryId;
            model.CatWordId = catWord.Id;
            model.TranslationIds = string.Join(",", translationIds);
            model.SetCommaTranslations(translations);
        }
    }
}