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

        /// <summary>
        /// Creates the Charts Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public ChartsController(DBDictionaryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the stats about the amount of words in each language
        /// and returns them to then display on the chart.
        /// </summary>
        /// <returns></returns>
        [HttpGet("JsonLangStats")]
        public JsonResult JsonLangStats()
        {
            List<object> wordList = new List<object>();
            wordList.Add(new[] { "Мова", "Кількість слів" });

            var languages = _context.Languages.Include(l => l.Words).ToList();
            foreach (var lang in languages)
            {
                wordList.Add(new object[] { lang.Name, lang.Words.Count });
            }

            return new JsonResult(wordList);
        }

        /// <summary>
        /// Gets the stats about the amount of words in each category
        /// and returns them to then display on the chart.
        /// </summary>
        /// <returns></returns>
        [HttpGet("JsonCatStats")]
        public JsonResult JsonCatStats()
        {
            List<object> wordList = new List<object>();
            wordList.Add(new[] { "Категорія", "Кількість слів" });

            var categories = _context.Categories.Include(c => c.CategorizedWords).ToList();
            foreach (var cat in categories)
            {
                wordList.Add(new object[] { cat.Name, cat.CategorizedWords.Count });
            }

            return new JsonResult(wordList);
        }

    }
}