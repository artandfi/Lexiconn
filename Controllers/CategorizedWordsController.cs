using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lexiconn;

namespace Lexiconn.Controllers
{
    public class CategorizedWordsController : Controller
    {
        private readonly DBDictionaryContext _context;

        public CategorizedWordsController(DBDictionaryContext context)
        {
            _context = context;
        }

        // GET: CategorizedWords
        public async Task<IActionResult> Index()
        {
            var dBDictionaryContext = _context.CategorizedWords.Include(c => c.Category).Include(c => c.Word);
            return View(await dBDictionaryContext.ToListAsync());
        }

        // GET: CategorizedWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorizedWord = await _context.CategorizedWords
                .Include(c => c.Category)
                .Include(c => c.Word)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorizedWord == null)
            {
                return NotFound();
            }

            return View(categorizedWord);
        }

        // GET: CategorizedWords/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["WordId"] = new SelectList(_context.Words, "Id", "ThisWord");
            return View();
        }

        // POST: CategorizedWords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WordId,CategoryId")] CategorizedWord categorizedWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categorizedWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categorizedWord.CategoryId);
            ViewData["WordId"] = new SelectList(_context.Words, "Id", "ThisWord", categorizedWord.WordId);
            return View(categorizedWord);
        }

        // GET: CategorizedWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorizedWord = await _context.CategorizedWords.FindAsync(id);
            if (categorizedWord == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categorizedWord.CategoryId);
            ViewData["WordId"] = new SelectList(_context.Words, "Id", "ThisWord", categorizedWord.WordId);
            return View(categorizedWord);
        }

        // POST: CategorizedWords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WordId,CategoryId")] CategorizedWord categorizedWord)
        {
            if (id != categorizedWord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categorizedWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategorizedWordExists(categorizedWord.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categorizedWord.CategoryId);
            ViewData["WordId"] = new SelectList(_context.Words, "Id", "ThisWord", categorizedWord.WordId);
            return View(categorizedWord);
        }

        // GET: CategorizedWords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorizedWord = await _context.CategorizedWords
                .Include(c => c.Category)
                .Include(c => c.Word)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorizedWord == null)
            {
                return NotFound();
            }

            return View(categorizedWord);
        }

        // POST: CategorizedWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorizedWord = await _context.CategorizedWords.FindAsync(id);
            _context.CategorizedWords.Remove(categorizedWord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategorizedWordExists(int id)
        {
            return _context.CategorizedWords.Any(e => e.Id == id);
        }
    }
}
