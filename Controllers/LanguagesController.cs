using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lexiconn.Models;

namespace Lexiconn.Controllers
{
    public class LanguagesController : Controller
    {
        private const string ERR_LANG_EXISTS = "Введена мова вже додана";
        private readonly DBDictionaryContext _context;

        /// <summary>
        /// Creates the Languages Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public LanguagesController(DBDictionaryContext context)
        {
            _context = context;
        }

        // GET: Languages
        /// <summary>
        /// Returns a list of available languages to display.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Languages.ToListAsync());
        }

        // GET: Languages/Details/*ID*
        /// <summary>
        /// Returns a detalized list of word contents for every language.
        /// </summary>
        /// <param name="langId">Selected language's ID.</param>
        public async Task<IActionResult> Details(int langId)
        {
            var modelList = new List<WordData>();
            var catWords = new List<CategorizedWord>();
            var words = await _context.Words.Where(w => w.LanguageId == langId).ToListAsync();
            var language = _context.Languages.Find(langId);

            foreach (var word in words)
            {
                var cw = await _context.CategorizedWords.Where(c => c.WordId == word.Id).ToListAsync();
                catWords.AddRange(cw);
            }
            
            foreach (var catWord in catWords)
            {
                var model = new WordData();
                FillModel(model, language, words, catWord);
                modelList.Add(model);
            }

            ViewData["LangId"] = langId;
            ViewData["Language"] = language.Name;
            return View(modelList);
        }

        /// <summary>
        /// Fills the word record model with all necessary information to store and display.
        /// </summary>
        /// <param name="model">The word record model.</param>
        /// <param name="language">The selected language.</param>
        /// <param name="words">A list of words in the selected language.</param>
        /// <param name="catWord">Current categorized word in the foreach loop.</param>
        private void FillModel(WordData model, Language language, List<Word> words, CategorizedWord catWord)
        {
            var category = _context.Categories.Find(catWord.CategoryId);
            var translations = _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToList();

            model.WordId = catWord.WordId;
            model.Word = words.Find(w => w.Id == catWord.WordId).ThisWord;
            model.LanguageId = language.Id;
            model.Language = language.Name;
            model.CategoryId = catWord.CategoryId;
            model.Category = catWord.Category.Name;
            model.CatWordId = catWord.Id;
            model.SetTranslationIds(translations);
            model.SetCommaTranslations(translations);
        }

        // GET: Languages/Create
        /// <summary>
        /// Returns a view for adding a new language.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        /// <summary>
        /// Adds the specified language to the database.
        /// </summary>
        /// <param name="language">A language to add.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Language language)
        {
            bool duplicate = await _context.Languages.AnyAsync(l => l.Name.Equals(language.Name));

            if (duplicate)
            {
                ModelState.AddModelError("Name", ERR_LANG_EXISTS);
            }

            if (ModelState.IsValid)
            {
                _context.Add(language);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        // GET: Languages/Edit/*ID*
        /// <summary>
        /// Returns the view with info of the language to edit.
        /// </summary>
        /// <param name="id">Selected language's ID.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }
            return View(language);
        }

        // POST: Languages/Edit/*ID*
        /// <summary>
        /// Updates the edited language if input was correct,
        /// displays an error message if it wasn't.
        /// </summary>
        /// <param name="id">Selected language's ID.</param>
        /// <param name="language">[Possibly] edited language.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Language language)
        {
            if (id != language.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/*ID*
        /// <summary>
        /// Removes the specified language from the database.
        /// </summary>
        /// <param name="id">The chosen language's ID.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Languages.FindAsync(id);
            _context.Languages.Remove(category);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Defines whether a language with specified ID is present in the database.
        /// </summary>
        /// <param name="id">Selected language's ID.</param>
        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}