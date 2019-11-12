using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DeveImageOptimizer.State.SqlState
{
    public class ProcessedFilesDbContext : DbContext
    {
        private readonly string _fileName;

        public DbSet<ProcessedFile> ProcessedFiles { get; set; }

        public ProcessedFilesDbContext(string fileName)
        {
            _fileName = fileName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedFile>()
                .HasIndex(b => b.Hash);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_fileName}");
        }
    }
}
