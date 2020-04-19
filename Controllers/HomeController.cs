using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lexiconn.Models;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Lexiconn.Controllers
{
    public class HomeController : Controller
    { 
        private readonly DBDictionaryContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly ClaimsPrincipal _user;

        /// <summary>
        /// Creates the Home Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public HomeController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
            _user = accessor.HttpContext.User;
        }

        /// <summary>
        /// Returns the view with all the word records to display on the home page.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var modelList = new List<WordData>();
            var catWords = await _context.CategorizedWords.Where(cw => cw.UserName.Equals(_user.Identity.Name)).ToListAsync();
            
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
            var word = _context.Words.Find(catWord.WordId);
            var language = _context.Languages.Find(word.LanguageId);
            var category = _context.Categories.Find(catWord.CategoryId);
            var translations = _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToList();

            WordDataHelper helper = new WordDataHelper(_context, _accessor);

            model.Word = word.ThisWord;
            model.WordId = catWord.WordId;
            model.Language = language.Name;
            model.LanguageId = word.LanguageId;
            model.Category = catWord.Category.Name ;
            model.CategoryId = catWord.CategoryId;
            model.CatWordId = catWord.Id;
            model.TranslationIds = helper.TranslationIdsToString(translations);
            model.Translation = helper.TranslationsToString(translations);
        }
    }
}