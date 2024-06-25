using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Entities;

namespace WebAPI.Infrastructure.DataContext
{
    public class AppDbContext : DbContext, IAppDbcontext
    {
         public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       

   
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    SeedRoles(modelBuilder);
        //}

        //private static void SeedRoles(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Role>().HasData
        //        (
        //        new Role { Id = 1, RoleCode = "Admin", RoleName = "Admin" },
        //        new Role { Id = 2, RoleCode = "User", RoleName = "User" }
        //        );
        //}

        public async Task<int> ComitChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
