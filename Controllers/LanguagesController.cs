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
    public class LanguagesController : Controller
    {
        private const string ERR_LANG_EXISTS = "Введена мова вже додана";
        private readonly DBDictionaryContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _accessor;

        public LanguagesController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
            _user = accessor.HttpContext.User;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Languages.Where(l => l.UserName.Equals(_user.Identity.Name)).ToListAsync());
        }

        public async Task<IActionResult> Details(int langId)
        {
            return await FillDetailsView(langId);
        }

        private async Task<ViewResult> FillDetailsView(int langId)
        {
            var language = _context.Languages.Find(langId);
            var words = await _context.Words.Where(w => w.LanguageId == langId).Include("CategorizedWords").ToListAsync();
            var modelList = new List<WordData>();

            FillViewData(langId, language.Name);
            FillModelList(modelList, language, words);

            return View(modelList);
        }

        private void FillViewData(int langId, string langName)
        {
            ViewData["LangId"] = langId;
            ViewData["Language"] = langName;
        }

        private void FillModelList(List<WordData> modelList, Language language, List<Word> words)
        {
            var catWords = new List<CategorizedWord>();

            FillCatWords(words, catWords);

            foreach (var catWord in catWords)
            {
                var model = new WordData();
                FillModel(model, language, words, catWord);
                modelList.Add(model);
            }
        }

        private void FillCatWords(List<Word> words, List<CategorizedWord> catWords)
        {
            foreach (var word in words)
            {
                catWords.AddRange(word.CategorizedWords);
            }
        }

        private void FillModel(WordData model, Language language, List<Word> words, CategorizedWord catWord)
        {
            var category = _context.Categories.Find(catWord.CategoryId);
            var translations = _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToList();
            var helper = new WordDataHelper(_context, _accessor);

            model.WordId = catWord.WordId;
            model.Word = words.Find(w => w.Id == catWord.WordId).ThisWord;
            model.LanguageId = language.Id;
            model.Language = language.Name;
            model.CategoryId = catWord.CategoryId;
            model.Category = catWord.Category.Name;
            model.CatWordId = catWord.Id;
            model.TranslationIds = helper.TranslationIdsToString(translations);
            model.Translation = helper.TranslationsToString(translations);
        }

        public IActionResult Create() => View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Language language)
        {
            bool duplicate = await _context.Languages.AnyAsync(l => l.Name.Equals(language.Name) && (l.UserName.Equals(_user.Identity.Name)));
            if (duplicate)
            {
                ModelState.AddModelError("Name", ERR_LANG_EXISTS);
            }

            if (ModelState.IsValid)
            {
                language.UserName = _user.Identity.Name;
                _context.Add(language);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Language language)
        {
            if (ModelState.IsValid)
            {
                if (!await UpdateLanguage(language))
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        private async Task<bool> UpdateLanguage(Language language)
        {
            try
            {
                _context.Update(language);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(language.Id))
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Languages.FindAsync(id);
            _context.Languages.Remove(category);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}