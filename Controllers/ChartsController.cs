using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DBDictionaryContext _context;

        public ChartsController(DBDictionaryContext context)
        {
            _context = context;
        }

        [HttpGet("JsonLangStats")]
        public JsonResult JsonLangStats()
        {
            var languages = _context.Languages.Include(l => l.Words).ToList();
            
            List<object> wordList = new List<object>();
            wordList.Add(new[] { "Мова", "Кількість слів" });

            foreach (var lang in languages)
            {
                wordList.Add(new object[] { lang.Name, lang.Words.Count });
            }

            return new JsonResult(wordList);
        }

        [HttpGet("JsonCatStats")]
        public JsonResult JsonCatStats()
        {
            var categories = _context.Categories.Include(c => c.CategorizedWords).ToList();

            List<object> wordList = new List<object>();
            wordList.Add(new[] { "Категорія", "Кількість слів" });

            foreach (var cat in categories)
            {
                wordList.Add(new object[] { cat.Name, cat.CategorizedWords.Count });
            }

            return new JsonResult(wordList);
        }

    }
}