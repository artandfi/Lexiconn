using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Lexiconn.Models;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    public class ReportsController : Controller
    {
        private const int WORD_IND = 1;
        private const int LANG_IND = 2;
        private const int CAT_IND  = 3;
        private const int TRAN_IND = 4;
        private const int MAX_LEN = 50;

        private const string ERR_EXT = "Оберіть Excel-файл";
        private const string ERR_FILE_NULL = "Файл відсутній";
        private const string ERR_DATA_NULL = "Файл містить запис із порожніми полями";
        private const string ERR_LEN_EXCD = "Файл містить запис із полями, довжина яких перевищує максимальну";
        private const string ERR_DUPLICATE = "Файл містить вже наявний запис";
        private const string ERR_TRAN = "Переклади наведено в некоректному форматі";
        private const string ERR_END = ". Будь ласка, спробуйте ще раз.";

        private readonly DBDictionaryContext _context;
        private readonly WordDataHelper _helper;

        public ReportsController(DBDictionaryContext context)
        {
            _context = context;
            _helper = new WordDataHelper(context);
        }

        public IActionResult Index(bool errorFlag, string error)
        {
            Report report = new Report();
            if (!errorFlag)
            {
                report.Error = ERR_EXT;
            }
            else
            {
                report.Error = error;
                report.ErrorPopupFlag = 1;
            }
            return View(report);
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
            return RedirectToAction("Index", "Home");
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
            string word = row.Cell(WORD_IND).Value.ToString();
            string lang = row.Cell(LANG_IND).Value.ToString();
            string cat = row.Cell(CAT_IND).Value.ToString();
            string translation = row.Cell(TRAN_IND).Value.ToString();
            WordData model = new WordData { Word = word, Language = lang, Category = cat, Translation = translation };

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

        private bool IsDuplicate(WordData model)
        {
            try
            {
                model.LanguageId = _context.Languages.First(l => l.Name.Equals(model.Language)).Id;
                model.CategoryId = _context.Categories.First(c => c.Name.Equals(model.Category)).Id;
            }
            catch (Exception e)
            {
                return false;
            }

            var word = _context.Words.FirstOrDefault(w => w.ThisWord.Equals(model.Word)
            && w.LanguageId == model.LanguageId);

            return word == null ? false : _context.CategorizedWords.Any(cw => cw.WordId == word.Id
            && cw.CategoryId == model.CategoryId);
        }

        private void CreateLanguage(WordData model)
        {
            Language language = new Language() { Name = model.Language };

            _context.Languages.Add(language);
            _context.SaveChanges();

            model.LanguageId = language.Id;
        }

        private void CreateCategory(WordData model)
        {
            Category category = new Category() { Name = model.Category };

            _context.Categories.Add(category);
            _context.SaveChanges();

            model.CategoryId = category.Id;
        }
    }
}