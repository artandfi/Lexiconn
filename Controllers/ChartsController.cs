using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DBDictionaryContext _context;
        private readonly ClaimsPrincipal _user;

        public ChartsController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        [HttpGet("JsonLangStats")]
        public JsonResult JsonLangStats()
        {
            var wordList = new List<object> { new[] { "Мова", "Кількість слів" } };
            var languages = _context.Languages.Where(t => t.UserName.Equals(_user.Identity.Name)).Include(t => t.Words).ToList();
            
            foreach (var lang in languages)
            {
                wordList.Add(new object[] { lang.Name, lang.Words.Count });
            }

            return new JsonResult(wordList);
        }

        [HttpGet("JsonCatStats")]
        public JsonResult JsonCatStats()
        {
            var wordList = new List<object> { new[] { "Категорія", "Кількість слів" } };
            var categories = _context.Categories.Where(c => c.UserName.Equals(_user.Identity.Name)).Include(c => c.CategorizedWords).ToList();
            
            foreach (var cat in categories)
            {
                wordList.Add(new object[] { cat.Name, cat.CategorizedWords.Count });
            }

            return new JsonResult(wordList);
        }
    }
}