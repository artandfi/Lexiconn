using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    public class WordDataController : Controller
    {
        // GET: WordData/Create
        public IActionResult Create()
        {
            var db = new DBDictionaryContext();

            if (!db.Languages.Any() || !db.Categories.Any())
            {
                SetDefaults(db);
            }

            ViewData["LanguageId"] = new SelectList(db.Languages, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: WordData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WordData model)
        {
            var db = new DBDictionaryContext();

            bool duplicate;
            var w = db.Words.FirstOrDefault(x => x.ThisWord.Equals(model.Word)
            && x.LanguageId == model.LanguageId);
            duplicate = w == null ? false : db.CategorizedWords.Any(cw => cw.WordId == w.Id
            && cw.CategoryId == model.CategoryId);

            if (duplicate)
            {
                ModelState.AddModelError("Translation", "Такий запис вже існує");
            }

            if (ModelState.IsValid)
            {
                var word = new Word();
                ProcessWord(word, db, model, out int wordId);

                var catWord = new CategorizedWord();
                ProcessCatWord(catWord, db, model, wordId, out int catWordId);

                // TODO: resolve multiple translations (add categorized word's PK to each one)
                var translation = new Translation();
                ProcessTranslation(translation, db, model, catWordId);

                return RedirectToAction("Create");
            }

            ViewData["LanguageId"] = new SelectList(db.Languages, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            return View(model);
        }

        // GET: WordData/Edit
        public IActionResult Edit(string word, int langId, int catId, string translation)
        {
            var db = new DBDictionaryContext();
            var model = new WordData();

            var wordId = db.Words.First(w => w.ThisWord.Equals(word)).Id;
            ViewData["WordId"] = wordId;
            var catWordId = db.CategorizedWords.First(cw => cw.WordId == wordId && cw.CategoryId == catId).Id;
            ViewData["CatWordId"] = catWordId;
            ViewData["LangId"] = langId;
            ViewData["CatId"] = catId;

            // TODO: multiple translations (split the translation string by commas, retrieve list of IDs)
            ViewData["TranslationId"] = db.Translations.First(t => t.ThisTranslation.Equals(translation) && t.CategorizedWordId == catWordId).Id;

            var langList = new SelectList(db.Languages, "Id", "Name");
            ViewData["LanguageList"] = langList;

            var catList = new SelectList(db.Categories, "Id", "Name");
            ViewData["CategoryList"] = catList;

            InitModel(model, db, langId, catId, word, translation);

            return View(model);
        }

        // POST: WordData/EditConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(string word, string translation, int langId, int catId, int wordId, int catWordId, int translationId)
        {
            var db = new DBDictionaryContext();

            if (ModelState.IsValid)
            {
                UpdateWord(word, wordId, langId, db);
                UpdateCatWord(catWordId, catId, db);
                UpdateTranslations(translationId, translation, db);

                return RedirectToAction("Index", "Home");
            }

            var model = new WordData();
            InitModel(model, db, langId, catId, word, translation);

            return View(model);
        }

        public IActionResult Delete(string word, int langId, int catId, string translation)
        {
            var model = new WordData();
            var db = new DBDictionaryContext();

            var wordEntity = db.Words.First(w => w.ThisWord.Equals(word) && w.LanguageId == langId);
            var catWordEntity = db.CategorizedWords.First(cw => cw.WordId == wordEntity.Id && cw.CategoryId == catId);
            // TODO: translations list (simply delete all)
            var translationEntity = db.Translations.First(t => t.CategorizedWordId == catWordEntity.Id);

            db.Remove(translationEntity);
            db.Remove(catWordEntity);
            db.SaveChanges();

            if (!db.CategorizedWords.Any(cw => cw.WordId == wordEntity.Id))
            {
                db.Remove(wordEntity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        private void ProcessWord(Word word, DBDictionaryContext db, WordData model, out int wordId)
        {
            word.LanguageId = model.LanguageId;
            word.ThisWord = model.Word;

            // Check if such word already exists in table, if it does, then resolve existing one's ID
            // (here, words are guaranteedly distinct)
            var sameWord = db.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word));

            if (sameWord == null)
            {
                db.Words.Add(word);
                db.SaveChanges();
                wordId = word.Id;
            }
            else
            {
                wordId = sameWord.Id;
            }
        }

        private void ProcessCatWord(CategorizedWord catWord, DBDictionaryContext db, WordData model, int wordId, out int catWordId)
        {
            catWord.WordId = wordId;
            catWord.CategoryId = model.CategoryId;

            db.CategorizedWords.Add(catWord);
            db.SaveChanges();
            catWordId = catWord.Id;
        }

        private void ProcessTranslation(Translation translation, DBDictionaryContext db, WordData model, int catWordId)
        {
            translation.CategorizedWordId = catWordId;
            translation.ThisTranslation = model.Translation;

            db.Translations.Add(translation);
            db.SaveChanges();
        }


        // In Words table, updates the word and its language.
        private void UpdateWord(string newWord, int wordId, int langId, DBDictionaryContext db)
        {
            var wordEntity = db.Words.First(w => w.Id == wordId);
            wordEntity.ThisWord = newWord;
            wordEntity.LanguageId = langId;

            db.Update(wordEntity);
            db.SaveChanges();
        }

        // In Categorized Words table, updates the categorized word's category ID.
        private void UpdateCatWord(int catWordId, int newCatId, DBDictionaryContext db)
        {
            var catWordEntity = db.CategorizedWords.First(cw => cw.Id == catWordId);
            catWordEntity.CategoryId = newCatId;

            db.Update(catWordEntity);
            db.SaveChanges();
        }

        // TODO: multiple translations update logic
        // In Translations table, updates the translation.
        private void UpdateTranslations(int translationId, string newTranslation, DBDictionaryContext db)
        {
            var translationEntity = db.Translations.First(t => t.Id == translationId);
            translationEntity.ThisTranslation = newTranslation;

            db.Update(translationEntity);
            db.SaveChanges();
        }

        private void SetDefaults(DBDictionaryContext db)
        {
            var catList = new List<Category>();
            catList.Add(new Category { Name = "Іменник" });
            catList.Add(new Category { Name = "Прикметник" });
            catList.Add(new Category { Name = "Числівник" });
            catList.Add(new Category { Name = "Займенник" });
            catList.Add(new Category { Name = "Дієслово" });
            catList.Add(new Category { Name = "Прислівник" });
            catList.Add(new Category { Name = "Сполучник" });
            catList.Add(new Category { Name = "Прийменник" });
            catList.Add(new Category { Name = "Частка" });
            catList.Add(new Category { Name = "Вигук" });
            catList.Add(new Category { Name = "Модальник" });

            db.Languages.Add(new Language { Name = "Англійська" });
            db.Categories.AddRange(catList);
            db.SaveChanges();
        }

        private void InitModel(WordData model, DBDictionaryContext db, int langId, int catId, string word, string translation)
        {
            model.LanguageId = langId;
            model.CategoryId = catId;
            model.Word = word;
            model.Language = db.Languages.First(l => l.Id == langId).Name;
            model.Category = db.Categories.First(c => c.Id == catId).Name;
            model.Translation = translation;
        }
    }
}