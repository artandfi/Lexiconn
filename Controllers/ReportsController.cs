using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Lexiconn.Models;
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

        private const string ERR_EXT = "Оберіть Excel-файл";
        private const string ERR_FILE_NULL = "Помилка читання файлу";
        private const string ERR_DATA_NULL = "Файл містить записи з порожніми полями";

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
                        foreach (IXLWorksheet worksheet in workbook.Worksheets)
                        {
                            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                            {
                                string word = row.Cell(WORD_IND).Value.ToString();
                                string lang = row.Cell(LANG_IND).Value.ToString();
                                string cat = row.Cell(CAT_IND).Value.ToString();
                                string translation = row.Cell(TRAN_IND).Value.ToString();

                                if (word.Equals("") || lang.Equals("") || cat.Equals("") || translation.Equals(""))
                                {
                                    ViewBag.Error = ERR_DATA_NULL;
                                    return RedirectToAction("Index", new { errorFlag = true });
                                }

                                // todo: use worddatacontroller create methods, make em static
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}