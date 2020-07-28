using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Lexiconn.Models;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    public class ReportsController : Controller
    {
        private const int WORD_IND = 1;
        private const int LANG_IND = 2;
        private const int CAT_IND  = 3;
        private const int TRAN_IND = 4;
        private const int MAX_LEN = 50;

        private const string REPORT_NAME = "Lexiconn – звіт";
        private const string REPORT_FORMAT = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private const string WORD = "Слово";
        private const string LANG = "Мова";
        private const string CAT = "Категорія";
        private const string TRAN = "Переклад";

        private const string WORD_CELL = "A1";
        private const string LANG_CELL = "B1";
        private const string CAT_CELL = "C1";
        private const string TRAN_CELL = "D1";

        private const string ERR_EXT = "Оберіть Excel-файл";
        private const string ERR_FILE_NULL = "Файл відсутній";
        private const string ERR_DATA_NULL = "Файл містить запис із порожнім полем";
        private const string ERR_LEN_EXCD = "Файл містить запис із полем, довжина якого перевищує максимальну";
        private const string ERR_DUPLICATE = "Файл містить вже наявний запис";
        private const string ERR_TRAN = "Переклади наведено в некоректному форматі";
        private const string ERR_END = ". Будь ласка, спробуйте ще раз.";
        private const string ERR_EXP_NIX = "Записи за вказаними фільтрами відсутні. Завантаження звіту відхилено.";

        private readonly DBDictionaryContext _context;
        private readonly WordDataHelper _helper;
        private readonly ClaimsPrincipal _user;

        public ReportsController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _helper = new WordDataHelper(context, accessor);
            _user = accessor.HttpContext.User;
        }

        public IActionResult Index(bool errorFlag, string error)
        {
            if (!errorFlag)
            {
                ViewBag.Error = ERR_EXT;
            }
            else
            {
                ViewBag.Error = error;
                ViewBag.ErrorPopupFlag = 1;
            }

            WordData model = new WordData();

            FillSelectLists();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (fileExcel != null)
            {
                using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                {
                    await fileExcel.CopyToAsync(stream);
                    using (XLWorkbook workbook = new XLWorkbook(stream, XLEventTracking.Disabled))
                    {
                        if (!ParseReport(workbook, out string error))
                        {
                            return RedirectToAction("Index", new { errorFlag = true, error = error });
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", new { errorFlag = true, error = ERR_FILE_NULL + ERR_END });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(WordData criteria)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet = workbook.Worksheets.Add(REPORT_NAME);
                var catWords = new List<CategorizedWord>();
                
                FormCatWordsToExport(criteria, ref catWords);

                if (catWords.Count == 0)
                {
                    return RedirectToAction("Index", new { errorFlag = true, error = ERR_EXP_NIX });
                }

                FillWorksheet(worksheet, catWords);
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    await stream.FlushAsync();

                    return new FileContentResult(stream.ToArray(), REPORT_FORMAT)
                    { FileDownloadName = $"Lexiconn {DateTime.Now.ToString()}.xlsx"};
                }
            }
        }

        private void FillSelectLists()
        {
            ViewBag.LanguageList = new SelectList(_context.Languages.Where(l => l.UserName.Equals(_user.Identity.Name)), "Id", "Name");
            ViewBag.CategoryList = new SelectList(_context.Categories.Where(c => c.UserName.Equals(_user.Identity.Name)), "Id", "Name");
        }

        private bool ParseReport(XLWorkbook workbook, out string error)
        {
            error = "";
            foreach (IXLWorksheet worksheet in workbook.Worksheets)
            {
                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    if (!ParseRow(row, out error))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ParseRow(IXLRow row, out string error)
        {
            var word = row.Cell(WORD_IND).Value.ToString();
            var lang = row.Cell(LANG_IND).Value.ToString();
            var cat = row.Cell(CAT_IND).Value.ToString();
            var translation = row.Cell(TRAN_IND).Value.ToString();
            var model = new WordData { Word = word, Language = lang, Category = cat, Translation = translation };

            if (IsRowErronous(model, out error))
            {
                return false;
            }

            CreateNewLangAndCatIfNeeded(model);
            _helper.CreateWord(model, out int wordId);
            _helper.CreateCatWord(model, wordId, out int catWordId);
            _helper.CreateTranslations(model, catWordId, out bool commaError);

            if (!_helper.ValidateComma(commaError, wordId))
            {
                error = ERR_TRAN + ERR_END;
                return false;
            }

            return true;
        }

        private bool IsRowErronous(WordData model, out string error)
        {
            error = "";

            if (model.Word.Equals("") || model.Language.Equals("")
                || model.Category.Equals("") || model.Translation.Equals(""))
            {
                error = ERR_DATA_NULL + ERR_END;
                return true;
            }

            if (model.Word.Length >= MAX_LEN || model.Language.Length >= MAX_LEN
                || model.Category.Length >= MAX_LEN || model.Translation.Length >= MAX_LEN)
            {
                error = ERR_LEN_EXCD + ERR_END;
                return true;
            }

            if (IsDuplicate(model))
            {
                error = ERR_DUPLICATE + ERR_END;
                return true;
            }

            return false;
        }

        private void CreateNewLangAndCatIfNeeded(WordData model)
        {
            if (model.LanguageId == 0)
            {
                CreateLanguage(model);
            }
            if (model.CategoryId == 0)
            {
                CreateCategory(model);
            }
        }

        private bool IsDuplicate(WordData model)
        {
            if (!ResolveNewLangCat(model))
            {
                return false;
            }

            var word = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word)
            && w.LanguageId == model.LanguageId);

            return word == null ? false : _context.CategorizedWords.Any(cw => cw.WordId == word.Id
            && cw.CategoryId == model.CategoryId);
        }

        private bool ResolveNewLangCat(WordData model)
        {
            var lang = _context.Languages.FirstOrDefault(l => l.Name.Equals(model.Language) && (l.UserName.Equals(_user.Identity.Name)));
            var cat = _context.Categories.FirstOrDefault(c => c.Name.Equals(model.Category) && (c.UserName.Equals(_user.Identity.Name)));

            if (lang == null)
            {
                if (cat != null)
                {
                    model.CategoryId = cat.Id;
                }
                return false;
            }
            else
            {
                model.LanguageId = lang.Id;
                if (cat == null)
                {
                    return false;
                }
                model.CategoryId = cat.Id;
            }
            return true;
        }

        private void CreateLanguage(WordData model)
        {
            var language = new Language() { Name = model.Language, UserName = _user.Identity.Name };

            _context.Languages.Add(language);
            _context.SaveChanges();

            model.LanguageId = language.Id;
        }

        private void CreateCategory(WordData model)
        {
            var category = new Category() { Name = model.Category, UserName = _user.Identity.Name };

            _context.Categories.Add(category);
            _context.SaveChanges();

            model.CategoryId = category.Id;
        }

        private void FormCatWordsToExport(WordData criteria, ref List<CategorizedWord> catWords)
        {
            if (criteria.LanguageId == 0)
            {
                catWords = criteria.CategoryId == 0 ?
                _context.CategorizedWords.Where(cw => cw.UserName.Equals(_user.Identity.Name)).ToList() :
                _context.CategorizedWords.Where(cw => cw.CategoryId == criteria.CategoryId).ToList();
            }
            else
            {
                var words = _context.Words.Where(w => w.LanguageId == criteria.LanguageId).Include("CategorizedWords").ToList();
                if (criteria.CategoryId == 0)
                {
                    foreach (var word in words)
                    {
                        catWords.AddRange(word.CategorizedWords);
                    }
                }
                else
                {
                    foreach (var word in words)
                    {
                        catWords.AddRange(word.CategorizedWords.Where(cw => cw.CategoryId == criteria.CategoryId));
                    }
                }
            }
        }

        private void FillWorksheet(IXLWorksheet worksheet, List<CategorizedWord> catWords)
        {
            worksheet.Cell(WORD_CELL).Value = WORD;
            worksheet.Cell(LANG_CELL).Value = LANG;
            worksheet.Cell(CAT_CELL).Value = CAT;
            worksheet.Cell(TRAN_CELL).Value = TRAN;
            worksheet.Row(1).Style.Font.Bold = true;

            for (int i = 0; i < catWords.Count; i++)
            {
                var word = _context.Words.Find(catWords[i].WordId);
                var language = _context.Languages.Find(word.LanguageId);
                var category = _context.Categories.Find(catWords[i].CategoryId);
                var translations = _context.Translations.Where(t => t.CategorizedWordId == catWords[i].Id).ToList();

                worksheet.Cell(i + 2, WORD_IND).Value = word.ThisWord;
                worksheet.Cell(i + 2, LANG_IND).Value = language.Name;
                worksheet.Cell(i + 2, CAT_IND).Value = category.Name;
                worksheet.Cell(i + 2, TRAN_IND).Value = _helper.TranslationsToString(translations);
            }
        }
    }
}