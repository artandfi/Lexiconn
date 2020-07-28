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
        private readonly ClaimsPrincipal _user;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(DBDictionaryContext context, UserManager<User> userManager, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
            _user = accessor.HttpContext.User;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            SetDefaults();
            return await FillView();
        }

        private void SetDefaults()
        {
            var userName = _user.Identity.Name;

            if (!_context.Languages.Any(l => l.UserName.Equals(userName)))
            {
                SetDefaultLanguages(userName);
            }
            if (!_context.Categories.Any(c => c.UserName.Equals(_user.Identity.Name)))
            {
                SetDefaultCategories(userName);
            }
        }

        private void SetDefaultLanguages(string userName)
        {
            if (userName != null)
            {
                _context.Languages.Add(new Language { Name = "Англійська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Російська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Німецька", UserName = userName });
                _context.Languages.Add(new Language { Name = "Іспанська", UserName = userName });
                _context.Languages.Add(new Language { Name = "Французька", UserName = userName });
                _context.SaveChanges();
            }
        }

        private void SetDefaultCategories(string userName)
        {
            if (userName != null)
            {
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

        private async Task<ViewResult> FillView()
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