using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lexiconn;
using Lexiconn.Models;

namespace Lexiconn.Controllers
{
    public class LanguagesController : Controller
    {
        private readonly DBDictionaryContext _context;

        public LanguagesController(DBDictionaryContext context)
        {
            _context = context;
        }

        // GET: Languages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Languages.ToListAsync());
        }

        // GET: Languages/Details/5
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

            ViewData["Language"] = language.Name;
            ViewData["LangId"] = langId;
            return View(modelList);
        }

        private void FillModel(WordData model, Language language, List<Word> words, CategorizedWord catWord)
        {
            model.Word = words.Find(w => w.Id == catWord.WordId).ThisWord;

            model.LanguageId = language.Id;
            model.Language = language.Name;

            var category = _context.Categories.Find(catWord.CategoryId);
            model.Category = catWord.Category.Name;
            model.CategoryId = catWord.CategoryId;

            var translations = _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToList();
            var translationIds = new List<int>();

            foreach (var translation in translations)
            {
                translationIds.Add(translation.Id);
            }

            model.TranslationIds = string.Join(",", translationIds);
            model.SetCommaTranslations(translations);
        }

        // GET: Languages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Language language)
        {
            bool duplicate = await _context.Languages.AnyAsync(l => l.Name.Equals(language.Name));

            if (duplicate)
            {
                ModelState.AddModelError("Name", "Введена мова вже додана");
            }

            if (ModelState.IsValid)
            {
                _context.Add(language);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        // GET: Languages/Edit/5
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

        // POST: Languages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Languages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _context.Languages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // POST: Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            var words = await _context.Words.Where(w => w.LanguageId == id).ToListAsync();
            var catWords = new List<CategorizedWord>();
            var translations = new List<Translation>();

            foreach (var word in words)
            {
                var cwList = await _context.CategorizedWords.Where(cw => cw.WordId == word.Id).ToListAsync();
                catWords.AddRange(cwList);
            }

            foreach (var catWord in catWords)
            {
                var trList = await _context.Translations.Where(t => t.CategorizedWordId == catWord.Id).ToListAsync();
                translations.AddRange(trList);
            }

            _context.Translations.RemoveRange(translations);
            _context.CategorizedWords.RemoveRange(catWords);
            _context.Words.RemoveRange(words);
            _context.Languages.Remove(language);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}
