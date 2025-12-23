using FxNet.Test.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FxNet.Test.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Journal> Journals => Set<Journal>();
        public DbSet<Tree> Trees => Set<Tree>();
        public DbSet<TreeNode> TreeNodes => Set<TreeNode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tree>()
                .HasIndex(t => new { t.Name })
                .IsUnique();

            modelBuilder.Entity<TreeNode>()
                .HasOne(n => n.ParentNode)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentNodeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
