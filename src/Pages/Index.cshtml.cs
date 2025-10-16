using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using aspnet_booklog.Data;
using aspnet_booklog.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnet_booklog.Pages
{
    public class IndexModel(AppDbContext db) : PageModel
    {
        private readonly AppDbContext _db = db;

        [BindProperty]
        public Book NewBook { get; set; } = new();

        public List<Book> Books { get; set; } = [];

        public async Task OnGet()
        {
            Books = await _db.Books.OrderByDescending(b => b.Id).ToListAsync();
        }

        public async Task<IActionResult> OnPostAdd()
        {
            if (!ModelState.IsValid) { await OnGet(); return Page(); }
            _db.Books.Add(NewBook);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var b = await _db.Books.FindAsync(id);
            if (b != null) { _db.Books.Remove(b); await _db.SaveChangesAsync(); }
            return RedirectToPage();
        }
    }
}
