using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Lexiconn.Controllers
{
    public class WordDataController : Controller
    {
        // GET: WordData/Create
        public IActionResult Create(string returnController, string returnAction, int langId)
        {
            var db = new DBDictionaryContext();

            if (!db.Languages.Any() || !db.Categories.Any())
            {
                SetDefaults(db);
            }

            ViewData["LangId"] = langId;
            ViewData["LanguageId"] = new SelectList(db.Languages, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            ViewData["ReturnController"] = returnController;
            ViewData["ReturnAction"] = returnAction;
            return View();
        }

        // POST: WordData/Create
        // REFACTOR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WordData model, string returnController, string returnAction, int langId)
        {
            var db = new DBDictionaryContext();

            if (IsDuplicate(model.Word, model.LanguageId, model.CategoryId, db))
            {
                ModelState.AddModelError("Translation", "Такий запис вже існує");
            }

            bool commaError = false;

            int wordId = 0;
            int catWordId = 0;

            if (ModelState.IsValid)
            {
                ProcessWord(db, model, out wordId);
                ProcessCatWord(db, model, wordId, out catWordId);
                ProcessTranslation(db, model, catWordId, out commaError);
            }
            else
            {
                ViewData["LangId"] = langId;
                ViewData["LanguageId"] = new SelectList(db.Languages, "Id", "Name");
                ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
                ViewData["ReturnController"] = returnController;
                ViewData["ReturnAction"] = returnAction;
                return View(model);
            }

            if (commaError)
            {
                ModelState.AddModelError("Translation", "Некоректний формат введення");
                var catWord = db.CategorizedWords.Find(catWordId);
                var word = db.Words.Find(wordId);
                db.CategorizedWords.Remove(catWord);
                db.Words.Remove(word);
                db.SaveChanges();
            }
            else
            {
                return returnController.Equals("Home") ? RedirectToAction("Create", new { returnController = returnController, returnAction = returnAction })
                    : RedirectToAction("Create", new { returnController = returnController, returnAction = returnAction, langId = langId });
            }

            ViewData["LangId"] = langId;
            ViewData["ReturnController"] = returnController;
            ViewData["ReturnAction"] = returnAction;
            ViewData["LanguageId"] = new SelectList(db.Languages, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            return View(model);
        }

        // GET: WordData/Edit
        public IActionResult Edit(string word, int langId, int catId, string translation, string translationIds, string returnController, string returnAction)
        {
            var db = new DBDictionaryContext();
            var model = new WordData();

            var wordId = db.Words.First(w => w.ThisWord.Equals(word) && w.LanguageId == langId).Id;
            var catWordId = db.CategorizedWords.First(cw => cw.WordId == wordId && cw.CategoryId == catId).Id;
            var langList = new SelectList(db.Languages, "Id", "Name");
            var catList = new SelectList(db.Categories, "Id", "Name");

            ViewData["WordId"] = wordId;
            ViewData["CatWordId"] = catWordId;
            ViewData["LangId"] = langId;
            ViewData["CatId"] = catId;
            ViewData["TranslationIds"] = translationIds;
            ViewData["LanguageList"] = langList;
            ViewData["CategoryList"] = catList;
            ViewData["OldTranslation"] = translation;
            ViewData["ReturnController"] = returnController;
            ViewData["ReturnAction"] = returnAction;

            InitModel(model, db, langId, catId, word, translation);
            return View(model);
        }

        // POST: WordData/EditConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WordData data, string word, string translation, int langId, int catId, int wordId, int catWordId, string translationIds, string returnController, string returnAction)
        {
            var db = new DBDictionaryContext();

            if (ModelState.IsValid)
            {
                UpdateWord(word, wordId, ref langId, db);
                UpdateCatWord(catWordId, catId, db);
                UpdateTranslations(translationIds, translation, catWordId, db);

                ViewData["ReturnAction"] = returnAction;
                ViewData["ReturnController"] = returnController;
                ViewData["LangId"] = langId;

                return RedirectToAction(returnAction, returnController, new { langId = langId });
            }

            var langList = new SelectList(db.Languages, "Id", "Name");
            var catList = new SelectList(db.Categories, "Id", "Name");

            ViewData["ReturnAction"] = returnAction;
            ViewData["ReturnController"] = returnController;
            ViewData["WordId"] = wordId;
            ViewData["CatWordId"] = catWordId;
            ViewData["LangId"] = langId;
            ViewData["CatId"] = catId;
            ViewData["TranslationIds"] = translationIds;
            ViewData["LanguageList"] = langList;
            ViewData["CategoryList"] = catList;

            var model = new WordData();
            InitModel(model, db, langId, catId, word, translation);

            return View(model);
        }

        public IActionResult Delete(string word, int langId, int catId, string returnController, string returnAction)
        {
            var model = new WordData();
            var db = new DBDictionaryContext();

            var wordEntity = db.Words.First(w => w.ThisWord.Equals(word) && w.LanguageId == langId);
            var catWordEntity = db.CategorizedWords.First(cw => cw.WordId == wordEntity.Id && cw.CategoryId == catId);
            var translationsEntity = db.Translations.Where(t => t.CategorizedWordId == catWordEntity.Id).ToList();

            db.RemoveRange(translationsEntity);
            db.Remove(catWordEntity);
            db.SaveChanges();

            if (!db.CategorizedWords.Any(cw => cw.WordId == wordEntity.Id))
            {
                db.Remove(wordEntity);
                db.SaveChanges();
            }

            ViewData["ReturnAction"] = returnAction;
            ViewData["ReturnController"] = returnController;
            ViewData["LangId"] = langId;

            return RedirectToAction(returnAction, returnController, new { langId = langId });
        }

        private void ProcessWord(DBDictionaryContext db, WordData model, out int wordId)
        {
            var word = new Word();
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

        private void ProcessCatWord(DBDictionaryContext db, WordData model, int wordId, out int catWordId)
        {
            var catWord = new CategorizedWord();
            catWord.WordId = wordId;
            catWord.CategoryId = model.CategoryId;

            db.CategorizedWords.Add(catWord);
            db.SaveChanges();
            catWordId = catWord.Id;
        }

        private void ProcessTranslation(DBDictionaryContext db, WordData model, int catWordId, out bool error)
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

            db.Translations.AddRange(translations);
            db.SaveChanges();
        }

        private bool SplitTranslations(string raw, ref List<Translation> translations)
        {
            string curTranslation = "";
            char cur = raw[0];

            if (!ResolveFirstCharacter(cur, ref curTranslation))
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
                curTranslation = curTranslation.Substring(0, curTranslation.Length - 1);

            translations.Add(new Translation() { ThisTranslation = curTranslation });
            return true;
        }

        private bool ResolveFirstCharacter(char cur, ref string curTranslation)
        {
            if (cur == ',' || cur == ' ')
            {
                return false;
            }
            
            curTranslation += cur;

            return true;
        }

        private bool ResolveCharacters(char prev, char cur, ref string curTranslation, ref List<Translation> translations)
        {
            if (cur == ',')
            {
                if (prev == ',')
                {
                    return false;
                }

                if (cur == ',')
                {
                    translations.Add(new Translation() { ThisTranslation = curTranslation });
                    curTranslation = "";
                }
            }
            else
            {
                if (cur != ' ' || cur == ' ' && prev != ' ' && prev != ',')
                    curTranslation += cur;
            }

            return true;
        }

        // In Words table, updates the word and its language.
        private void UpdateWord(string newWord, int wordId, ref int langId, DBDictionaryContext db)
        {
            var wordEntity = db.Words.First(w => w.Id == wordId);
            wordEntity.ThisWord = newWord;
            
            if (wordEntity.LanguageId != langId)
            {
                int tmp = wordEntity.LanguageId;
                wordEntity.LanguageId = langId;
                langId = tmp;
            }

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

        private bool UpdateTranslations(string translationIdList, string newTranslation, int catWordId, DBDictionaryContext db)
        {
            var oldTranslationIds = translationIdList.Split(',').Select(Int32.Parse).ToList();
            var oldTranslations = new List<Translation>();
            var newTranslations = new List<Translation>();

            if (!SplitTranslations(newTranslation, ref newTranslations))
            {
                return false;
            }

            for (int i = 0; i < oldTranslationIds.Count; i++)
            {
                oldTranslations.Add(db.Translations.Find(oldTranslationIds[i]));
                if (i < newTranslations.Count)
                    oldTranslations[i].ThisTranslation = newTranslations[i].ThisTranslation;
            }

            for (int i = 0; i < newTranslations.Count; i++)
            {
                newTranslations[i].CategorizedWordId = catWordId;
            }

            ResolveTranslationUpdate(oldTranslations, newTranslations, oldTranslationIds);
            return true;
        }

        private void ResolveTranslationUpdate(List<Translation> oldTs, List<Translation> newTs, List<int> oldIds)
        {
            var db = new DBDictionaryContext();
            // Update old, add new
            if (oldTs.Count < newTs.Count)
            {
                for (int i = 0; i < oldTs.Count; i++)
                {
                    db.Update(oldTs[i]);
                }
                for (int i = oldTs.Count; i < newTs.Count; i++)
                {
                    db.Translations.Add(newTs[i]);
                }
            }
            // Delete extra if present, update old
            else
            {
                for (int i = newTs.Count; i < oldTs.Count; i++)
                {
                    db.Translations.Remove(oldTs[i]);
                }

                for (int i = 0; i < newTs.Count; i++)
                {
                    db.Update(oldTs[i]);
                }
            }
            db.SaveChanges();
        }

        private bool IsDuplicate(string word, int langId, int catId, DBDictionaryContext db)
        {
            var w = db.Words.FirstOrDefault(x => x.ThisWord.Equals(word)
            && x.LanguageId == langId);

            return w == null ? false : db.CategorizedWords.Any(cw => cw.WordId == w.Id
            && cw.CategoryId == catId);
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