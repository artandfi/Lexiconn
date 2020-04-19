using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lexiconn.Models;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Lexiconn.Controllers
{
    public class HomeController : Controller
    { 
        private readonly DBDictionaryContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Creates the Home Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public HomeController(DBDictionaryContext context, UserManager<User> userManager, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
            _user = accessor.HttpContext.User;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns the view with all the word records to display on the home page.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            if (_userManager.Users.Count() == 1)
            {
                if (!_context.Languages.Any())
                {
                    SetDefaultLanguagesAdmin();
                }
                if (!_context.Categories.Any())
                {
                    SetDefaultCategoriesAdmin();
                }
            }
            else
            {
                if (!_context.Languages.Any(l => l.UserName.Equals(_user.Identity.Name)))
                {
                    SetDefaultLanguages();
                }
                if (!_context.Categories.Any(c => c.UserName.Equals(_user.Identity.Name)))
                {
                    SetDefaultCategories();
                }
            }
            

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

        /// <summary>
        /// Sets the default languages when there is no languages.
        /// </summary>
        private void SetDefaultLanguages()
        {
            if (_user.Identity.Name != null)
            {
                var userName = _user.Identity.Name;
                _context.Languages.Add(new Language { Name = "Англійська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Російська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Німецька", UserName = userName });
                _context.Languages.Add(new Language { Name = "Іспанська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Французька", UserName = userName });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the default categories when there is no categories.
        /// </summary>
        private void SetDefaultCategories()
        {
            if (_user.Identity.Name != null)
            {
                var userName = _user.Identity.Name;
                _context.Categories.Add(new Category { Name = "Іменник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Прикметник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Числівник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Займенник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Дієслово", UserName = userName });
                _context.Categories.Add(new Category { Name = "Прислівник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Сполучник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Прийменник", UserName = userName });
                _context.Categories.Add(new Category { Name = "Частка", UserName = userName });
                _context.Categories.Add(new Category { Name = "Вигук", UserName = userName });
                _context.Categories.Add(new Category { Name = "Модальник", UserName = userName });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the default languages when there is no languages.
        /// </summary>
        private void SetDefaultLanguagesAdmin()
        {
            var userName = "admin";
            _context.Languages.Add(new Language { Name = "Англійська", UserName = userName });
            _context.Languages.Add(new Language { Name = "Російська", UserName = userName });
            _context.Languages.Add(new Language { Name = "Німецька", UserName = userName });
            _context.Languages.Add(new Language { Name = "Іспанська", UserName = userName });
            _context.Languages.Add(new Language { Name = "Французька", UserName = userName });
            _context.SaveChanges();
        }

        /// <summary>
        /// Sets the default categories when there is no categories.
        /// </summary>
        private void SetDefaultCategoriesAdmin()
        {
            var userName = "admin";
            _context.Categories.Add(new Category { Name = "Іменник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Прикметник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Числівник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Займенник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Дієслово", UserName = userName });
            _context.Categories.Add(new Category { Name = "Прислівник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Сполучник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Прийменник", UserName = userName });
            _context.Categories.Add(new Category { Name = "Частка", UserName = userName });
            _context.Categories.Add(new Category { Name = "Вигук", UserName = userName });
            _context.Categories.Add(new Category { Name = "Модальник", UserName = userName });
            _context.SaveChanges();
        }
    }
}