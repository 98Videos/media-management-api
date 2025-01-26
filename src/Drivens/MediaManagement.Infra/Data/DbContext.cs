using MediaManagement.Database.ModelConfiguration;
using MediaManagementApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaManagement.Database.Data;

public class VideoDbContext: DbContext
{
        public DbSet<Video> Video { get; set; }
        public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
                new VideoModelConfiguration().Configure(modelBuilder.Entity<Video>());
        }
}