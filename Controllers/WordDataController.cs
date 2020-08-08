using System;
using System.Collections.Generic;
using System.Linq;
using Lexiconn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Lexiconn.Controllers
{
    public class WordDataController : Controller
    {
        private const string ERR_REC_EXISTS = "Такий запис вже існує";
        private const string ERR_INPUT = "Некоректний формат перекладів";

        private readonly DBDictionaryContext _context;
        private readonly WordDataHelper _helper;
        private readonly ClaimsPrincipal _user;

        public WordDataController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _helper = new WordDataHelper(context, accessor);
            _user = accessor.HttpContext.User;
        }

        public IActionResult Create(int langId, string returnController, string returnAction)
        {
            FillReturnPath(returnController, returnAction);
            FillSelectLists(langId);
            return View(new WordData());
        }

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

        public IActionResult Edit(WordData model, string returnController, string returnAction)
        {
            ViewBag.LangId = model.LanguageId;
            FillSelectLists(model.LanguageId);
            FillReturnPath(returnController, returnAction);
            return View(model);
        }

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

            return catWord != null && catWord.Id != model.CatWordId;
        }

        private void FillReturnPath(string returnController, string returnAction)
        {
            ViewBag.ReturnController = returnController;
            ViewBag.ReturnAction = returnAction;
        }

        private void FillSelectLists(int langId)
        {
            var langList = langId == 0 ?
                new SelectList(_context.Languages.Where(l => l.UserName.Equals(_user.Identity.Name)), "Id", "Name") :
                new SelectList(_context.Languages.Where(l => l.Id == langId), "Id", "Name");
            ViewBag.LanguageList = langList;
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.UserName.Equals(_user.Identity.Name)), "Id", "Name");
            ViewBag.LangId = langId;
        }
    }
}