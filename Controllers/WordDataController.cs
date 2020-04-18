using System;
using System.Collections.Generic;
using System.Linq;
using Lexiconn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lexiconn.Supplementary;

namespace Lexiconn.Controllers
{
    public class WordDataController : Controller
    {
        private const string ERR_REC_EXISTS = "Такий запис вже існує";
        private const string ERR_INPUT = "Некоректний формат перекладів";

        private readonly DBDictionaryContext _context;
        private readonly WordDataHelper _helper;

        /// <summary>
        /// Creates the Word Data Controller and provides it with a database context and a helper object.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public WordDataController(DBDictionaryContext context)
        {
            _context = context;
            _helper = new WordDataHelper(context);
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
            int langId = returnController.Equals("Languages") ? model.LanguageId : 0;

            FillReturnPath(returnController, returnAction);
            FillSelectLists(langId);

            if (IsDuplicate(model))
            {
                ModelState.AddModelError("Translation", ERR_REC_EXISTS);
            }

            if (ModelState.IsValid)
            {
                _helper.CreateWord(model, out int wordId);
                _helper.CreateCatWord(model, wordId, out int catWordId);
                _helper.CreateTranslations(model, catWordId, out bool commaError);
                
                if (_helper.ValidateComma(commaError, wordId))
                {
                    return RedirectToAction("Create", new { langId = langId, returnController = returnController, returnAction = returnAction });
                }

                ModelState.AddModelError("Translation", ERR_INPUT);
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
            FillSelectLists(model.LanguageId);

            if (ModelState.IsValid)
            {
                if (IsUpdateDuplicate(model))
                {
                    ModelState.AddModelError("Translation", ERR_REC_EXISTS);
                }
                else
                {
                    if (_helper.UpdateTranslations(model))
                    {
                        _helper.UpdateWord(model);
                        _helper.UpdateCatWord(model);
                        return RedirectToAction(returnAction, returnController, new { langId = model.LanguageId });
                    }
                    ModelState.AddModelError("Translation", ERR_INPUT);
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

            if (_context.CategorizedWords.Count(cw => cw.WordId == wordEntity.Id) == 1)
            {
                _context.Remove(wordEntity);
            }
            else
            {
                _context.Remove(catWordEntity);
            }
            _context.SaveChanges();
            
            ViewBag.LangId = model.LanguageId;
            ViewBag.Language = _context.Languages.First(l => l.Id == model.LanguageId).Name;
            FillReturnPath(returnController, returnAction);
            return RedirectToAction(returnAction, returnController, new { langId = model.LanguageId });
        }

        /// <summary>
        /// Determines whether the word record duplicates already existing one.
        /// Two records are considered duplicates if their words, languages and categories match.
        /// </summary>
        /// <param name="model">The word record to check.</param>
        private bool IsDuplicate(WordData model)
        {
            var word = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word)
            && w.LanguageId == model.LanguageId);

            return word == null ? false : _context.CategorizedWords.Any(cw => cw.WordId == word.Id
            && cw.CategoryId == model.CategoryId);
        }

        private bool IsUpdateDuplicate(WordData model)
        {
            var word = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word)
            && w.LanguageId == model.LanguageId);
            
            if (word == null)
            {
                return false;
            }

            var catWord = _context.CategorizedWords.FirstOrDefault(cw => cw.WordId == word.Id
            && cw.CategoryId == model.CategoryId);

            return catWord.Id != model.CatWordId;
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
            _context.Languages.Add(new Language { Name = "Німецька" });
            _context.Languages.Add(new Language { Name = "Іспанська" });
            _context.Languages.Add(new Language { Name = "Французька" });
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