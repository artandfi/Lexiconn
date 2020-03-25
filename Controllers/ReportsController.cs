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
        public IActionResult Index()
        {
            return View();
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
                                
                            }
                        }
                        
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}