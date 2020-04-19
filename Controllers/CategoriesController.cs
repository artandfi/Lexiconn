using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lexiconn.Controllers
{
    public class CategoriesController : Controller
    {
        private const string ERR_CAT_EXISTS = "Введена категорія вже додана";
        private readonly DBDictionaryContext _context;
        private readonly ClaimsPrincipal _user;

        /// <summary>
        /// Creates the Categories Controller and provides it with a database context.
        /// </summary>
        /// <param name="context">An object to interact with the database.</param>
        public CategoriesController(DBDictionaryContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        // GET: Categories
        /// <summary>
        /// Returns a list of available categories to display.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.Where(c => c.UserName.Equals(_user.Identity.Name)).ToListAsync());
        }

        // GET: Categories/Details/*ID*
        /// <summary>
        /// Returns a detalized list of word contents for every category.
        /// </summary>
        /// <param name="id">Selected category's ID.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        /// <summary>
        /// Returns a view for adding a new category.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        /// <summary>
        /// Adds the specified category to the database.
        /// </summary>
        /// <param name="category">A category to add.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            bool duplicate = await _context.Categories.AnyAsync(c => c.Name.Equals(category.Name) && (c.UserName.Equals(_user.Identity.Name)));

            if (duplicate)
            {
                ModelState.AddModelError("Name", ERR_CAT_EXISTS);
            }

            if (ModelState.IsValid)
            {
                category.UserName = _user.Identity.Name;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/*ID*
        /// <summary>
        /// Returns the view with info of the category to edit.
        /// </summary>
        /// <param name="id">Selected category's ID.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/*ID*
        /// <summary>
        /// Updates the edited category if input was correct,
        /// displays an error message if it wasn't.
        /// </summary>
        /// <param name="id">Selected category's ID.</param>
        /// <param name="category">[Possibly] edited category.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/*ID*
        /// <summary>
        /// Removes the specified category from the database.
        /// </summary>
        /// <param name="id">The chosen category's ID.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Defines whether a category with specified ID is present in the database.
        /// </summary>
        /// <param name="id">Selected category's ID.</param>
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}