using Microsoft.EntityFrameworkCore;
using aspnet_booklog.Models;
using System.Collections.Generic;

namespace aspnet_booklog.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options)
    {
        public DbSet<Book> Books => Set<Book>();
    }
}
