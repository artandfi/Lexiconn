using System;
using System.Collections.Generic;
using System.Linq;
using Lexiconn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lexiconn.Controllers
{
    public class WordDataController : Controller
    {
        private const string ERR_REC_EXISTS = "Такий запис вже існує";
        private const string ERR_INPUT = "Некоректний формат введення";

        private readonly DBDictionaryContext _context;

        public WordDataController(DBDictionaryContext context)
        {
            _context = context;
        }

        // GET: WordData/Create
        /// <summary>
        /// Returns a view for creating a new word record,
        /// setting default categories and languages if ones absent.
        /// </summary>
        /// <param name="langId">Language's ID needed to know if adding words from a certain language's page.
        /// Equals to zero if creating from the home page.</param>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        public IActionResult Create(int langId, string returnController, string returnAction)
        {
            if (!_context.Languages.Any())
            {
                SetDefaultLanguages();
            }
            if (!_context.Categories.Any())
            {
                SetDefaultCategories();
            }

            FillReturnPath(returnController, returnAction);
            FillSelectLists(langId);

            WordData model = new WordData();
            return View(model);
        }

        // POST: WordData/Create
        /// <summary>
        /// Adds a new word record to the database and redirects back here to maybe add another record if input valid.
        /// Displays the error if it isn't.
        /// </summary>
        /// <param name="model">The word record to add.</param>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WordData model, string returnController, string returnAction)
        {
            FillReturnPath(returnController, returnAction);
            FillSelectLists(model.LanguageId);

            if (IsDuplicate(model))
            {
                ModelState.AddModelError("Translation", ERR_REC_EXISTS);
            }

            if (ModelState.IsValid)
            {
                CreateWord(model, out int wordId);
                CreateCatWord(model, wordId, out int catWordId);
                CreateTranslations(model, catWordId, out bool commaError);
                
                if (ValidateComma(commaError, wordId))
                {
                    return RedirectToAction("Create", new { langId = model.LanguageId, returnController = returnController, returnAction = returnAction });
                }
            }

            return View(model);
        }

        // GET: WordData/Edit
        /// <summary>
        /// Returns a view for editing an existing word record.
        /// </summary>
        /// <param name="model">The word record to edit.</param>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        public IActionResult Edit(WordData model, string returnController, string returnAction)
        {
            ViewBag.LangId = model.LanguageId;
            FillSelectLists(model.LanguageId);
            FillReturnPath(returnController, returnAction);
            return View(model);
        }

        // POST: WordData/EditConfirmed
        /// <summary>
        /// Edits the record specified if new input valid.
        /// Displays the error if it isn't.
        /// </summary>
        /// <param name="model">The word record to edit.</param>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(WordData model, string returnController, string returnAction)
        {
            ViewBag.LangId = model.LanguageId;
            FillReturnPath(returnController, returnAction);

            if (ModelState.IsValid)
            {
                if (UpdateTranslations(model))
                {
                    UpdateWord(model);
                    UpdateCatWord(model);
                    return RedirectToAction(returnAction, returnController, new { langId = model.LanguageId });
                }
            }

            return View(model);
        }

        /// <summary>
        /// Removes the specified word record from the database,
        /// leaving ther word if there are more than one category for it and deleting it otherwise.
        /// </summary>
        /// <param name="model">The word record to remove.</param>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        public IActionResult Delete(WordData model, string returnController, string returnAction)
        {
            var wordEntity = _context.Words.First(w => w.ThisWord.Equals(model.Word) && w.LanguageId == model.LanguageId);
            var catWordEntity = _context.CategorizedWords.First(cw => cw.WordId == wordEntity.Id && cw.CategoryId == model.CategoryId);

            _context.Remove(catWordEntity);

            if (!_context.CategorizedWords.Any(cw => cw.WordId == wordEntity.Id))
            {
                _context.Remove(wordEntity);
            }
            _context.SaveChanges();
            
            ViewBag.LangId = model.LanguageId;
            ViewBag.Language = _context.Languages.First(l => l.Id == model.LanguageId).Name;
            FillReturnPath(returnController, returnAction);
            return RedirectToAction(returnAction, returnController, new { langId = model.LanguageId });
        }

        /// <summary>
        /// Creates a new word in the corresponding table if it isn't present there yet,
        /// gives away the existing one's ID otherwise, for the specified ford record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="wordId">The word's future or actual ID.</param>
        private void CreateWord(WordData model, out int wordId)
        {
            var word = new Word();
            var sameWord = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word));
            word.LanguageId = model.LanguageId;
            word.ThisWord = model.Word;

            if (sameWord == null)
            {
                _context.Words.Add(word);
                _context.SaveChanges();
                wordId = word.Id;
            }
            else
            {
                wordId = sameWord.Id;
            }
        }

        /// <summary>
        /// Adds a new categorized word in the corresponding table,
        /// giving away its ID, for the specified word record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="wordId">The added word's ID.</param>
        /// <param name="catWordId">The categorized word's ID to give away.</param>
        private void CreateCatWord(WordData model, int wordId, out int catWordId)
        {
            var catWord = new CategorizedWord();
            catWord.WordId = wordId;
            catWord.CategoryId = model.CategoryId;

            _context.CategorizedWords.Add(catWord);
            _context.SaveChanges();
            catWordId = catWord.Id;
        }

        /// <summary>
        /// Adds new translations to the corresponding table,
        /// giving away a possibly occured input format error flag,
        /// for the specified word record.
        /// </summary>
        /// <param name="model">The word record to add to the database.</param>
        /// <param name="catWordId">The added categorized word's ID.</param>
        /// <param name="error">The error flag to give away.</param>
        private void CreateTranslations(WordData model, int catWordId, out bool error)
        {
            error = false;
            List<Translation> translations = new List<Translation>();

            if (!SplitTranslations(model.Translation, ref translations))
            {
                error = true;
                return;
            }
            
            foreach (var translation in translations)
            {
                translation.CategorizedWordId = catWordId;
            }

            _context.Translations.AddRange(translations);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the word and its language for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        private void UpdateWord(WordData model)
        {
            var wordEntity = _context.Words.First(w => w.Id == model.WordId);
            wordEntity.ThisWord = model.Word;

            _context.Update(wordEntity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the categorized word's category for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        private void UpdateCatWord(WordData model)
        {
            var catWordEntity = _context.CategorizedWords.First(cw => cw.Id == model.CatWordId);
            catWordEntity.CategoryId = model.CategoryId;

            _context.Update(catWordEntity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates translations for the specified word record.
        /// </summary>
        /// <param name="model">The word record to update.</param>
        private bool UpdateTranslations(WordData model)
        {
            var oldTranslationIds = model.TranslationIds.Split(',').Select(Int32.Parse).ToList();
            var oldTranslations = new List<Translation>();
            var newTranslations = new List<Translation>();

            if (!SplitTranslations(model.Translation, ref newTranslations))
            {
                ModelState.AddModelError("Translation", ERR_INPUT);
                return false;
            }

            for (int i = 0; i < oldTranslationIds.Count; i++)
            {
                oldTranslations.Add(_context.Translations.Find(oldTranslationIds[i]));
                if (i < newTranslations.Count)
                    oldTranslations[i].ThisTranslation = newTranslations[i].ThisTranslation;
            }

            for (int i = 0; i < newTranslations.Count; i++)
            {
                newTranslations[i].CategorizedWordId = model.CatWordId;
            }

            ResolveTranslationUpdate(oldTranslations, newTranslations);
            return true;
        }

        /// <summary>
        /// Turns string-listed translations into a list of translations object.
        /// </summary>
        /// <param name="raw">The string-listed translations.</param>
        /// <param name="translations">List of translations object to fill in and give away.</param>
        private bool SplitTranslations(string raw, ref List<Translation> translations)
        {
            string curTranslation = "";
            char cur = raw[0];

            if (!IsFirstCharacterCorrect(cur, ref curTranslation))
            {
                return false;
            }

            for (int i = 1; i < raw.Length; i++)
            {
                cur = raw[i];
                char prev = raw[i - 1];

                if (!ResolveCharacters(prev, cur, ref curTranslation, ref translations))
                {
                    return false;
                }
            }

            if (curTranslation.Last() == ' ')
                curTranslation = curTranslation[0..^1];

            translations.Add(new Translation() { ThisTranslation = curTranslation });
            return true;
        }

        /// <summary>
        /// Determines whether a first character input is correct,
        /// i.e. whether it's not a comma or a whitespace. 
        /// </summary>
        /// <param name="cur">Current character read.</param>
        /// <param name="curTranslation">Current translation read.</param>
        private bool IsFirstCharacterCorrect(char cur, ref string curTranslation)
        {
            curTranslation += cur;
            return cur != ',' && cur != ' ';
        }

        /// <summary>
        /// Defines the correctness of all further characters and forges them into translations
        /// as they appear and if they are correct.
        /// </summary>
        /// <param name="prev">Previous character read.</param>
        /// <param name="cur">Current character read.</param>
        /// <param name="curTranslation">Current translation read.</param>
        /// <param name="translations">Translations' list to give away.</param>
        private bool ResolveCharacters(char prev, char cur, ref string curTranslation, ref List<Translation> translations)
        {
            if (cur == ',')
            {
                if (prev == ',')
                {
                    return false;
                }

                translations.Add(new Translation() { ThisTranslation = curTranslation });
                curTranslation = "";
            }
            else
            {
                if (cur != ' ' || cur == ' ' && prev != ' ' && prev != ',')
                {
                    curTranslation += cur;
                }
            }
            return true;
        }

        /// <summary>
        /// Resolves translations' update type:
        /// - if their amount increased, the existing are updated and the oncome added;
        /// - if their amount didn't change, they are updated;
        /// - if their amount decreased, the remaining are updated and the extra are removed.
        /// </summary>
        /// <param name="oldTs"></param>
        /// <param name="newTs"></param>
        private void ResolveTranslationUpdate(List<Translation> oldTs, List<Translation> newTs)
        {
            if (oldTs.Count < newTs.Count)
            {
                for (int i = 0; i < oldTs.Count; i++)
                {
                    _context.Update(oldTs[i]);
                }
                for (int i = oldTs.Count; i < newTs.Count; i++)
                {
                    _context.Translations.Add(newTs[i]);
                }
            }
            else
            {
                for (int i = newTs.Count; i < oldTs.Count; i++)
                {
                    _context.Translations.Remove(oldTs[i]);
                }

                for (int i = 0; i < newTs.Count; i++)
                {
                    _context.Update(oldTs[i]);
                }
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Determines whether an extra comma error was encountered during reading translations.
        /// If so, the creation of new word record is rolled back.
        /// </summary>
        /// <param name="commaError">A flag of extra comma error.</param>
        /// <param name="wordId">The ID of recently added word to delete.</param>
        /// <returns></returns>
        private bool ValidateComma(bool commaError, int wordId)
        {
            if (commaError)
            {
                ModelState.AddModelError("Translation", ERR_INPUT);
                
                var word = _context.Words.Find(wordId);
                _context.Words.Remove(word);
                _context.SaveChanges();
                
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the word record duplicates already existing one.
        /// Two records are considered duplicates if their words, languages and categories match.
        /// </summary>
        /// <param name="model">The word record to check.</param>
        private bool IsDuplicate(WordData model)
        {
            var w = _context.Words.FirstOrDefault(x => x.ThisWord.Equals(model.Word)
            && x.LanguageId == model.LanguageId);

            return w == null ? false : _context.CategorizedWords.Any(cw => cw.WordId == w.Id
            && cw.CategoryId == model.CategoryId);
        }

        /// <summary>
        /// Specifies the return path for redirects.
        /// </summary>
        /// <param name="returnController">Name of the controller of the last page visited.</param>
        /// <param name="returnAction">Name of action of the last page visited.</param>
        private void FillReturnPath(string returnController, string returnAction)
        {
            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
        }

        /// <summary>
        /// Provides the dropdown lists for languages and categories.
        /// If there was a language chosen to create a word record in (id is non-zero),
        /// then only this language is added to list to display.
        /// </summary>
        /// <param name="langId">Zero if there was no language chosen, its ID otherwise.</param>
        private void FillSelectLists(int langId)
        {
            var langList = langId == 0 ?
                new SelectList(_context.Languages, "Id", "Name") :
                new SelectList(_context.Languages.Where(l => l.Id == langId), "Id", "Name");
            ViewBag.LanguageList = langList;
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.LangId = langId;
        }

        /// <summary>
        /// Sets the default languages when there is no languages.
        /// </summary>
        private void SetDefaultLanguages()
        {
            _context.Languages.Add(new Language { Name = "Англійська" });
            _context.Languages.Add(new Language { Name = "Російська" });
        }

        /// <summary>
        /// Sets the default categories when there is no categories.
        /// </summary>
        private void SetDefaultCategories()
        {
            _context.Categories.Add(new Category { Name = "Іменник" });
            _context.Categories.Add(new Category { Name = "Прикметник" });
            _context.Categories.Add(new Category { Name = "Числівник" });
            _context.Categories.Add(new Category { Name = "Займенник" });
            _context.Categories.Add(new Category { Name = "Дієслово" });
            _context.Categories.Add(new Category { Name = "Прислівник" });
            _context.Categories.Add(new Category { Name = "Сполучник" });
            _context.Categories.Add(new Category { Name = "Прийменник" });
            _context.Categories.Add(new Category { Name = "Частка" });
            _context.Categories.Add(new Category { Name = "Вигук" });
            _context.Categories.Add(new Category { Name = "Модальник" });
            _context.SaveChanges();
        }
    }
}