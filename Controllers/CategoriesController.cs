using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    public class CategoriesController : Controller
    {
        private const string ERR_CAT_EXISTS = "Введена категорія вже додана";
        private readonly DBDictionaryContext _context;
        private readonly ClaimsPrincipal _user;

        public CategoriesController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.Where(c => c.UserName.Equals(_user.Identity.Name)).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            bool duplicate = await _context.Categories.AnyAsync(c => c.Name.Equals(category.Name) && (c.UserName.Equals(_user.Identity.Name)));
            if (duplicate)
            {
                ModelState.AddModelError("Name", ERR_CAT_EXISTS);
            }

            if (ModelState.IsValid)
            {
                CreateCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        private async void CreateCategory(Category category)
        {
            category.UserName = _user.Identity.Name;
            _context.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                if (!await UpdateCategory(category))
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        private async Task<bool> UpdateCategory(Category category)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}