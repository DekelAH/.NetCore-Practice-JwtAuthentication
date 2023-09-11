using CitiesManager.Core.Entities;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        #region Properties

        public virtual DbSet<City> Cities { get; set; }

        #endregion

        #region Ctors

        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public ApplicationDbContext() { }

        #endregion

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData(new City()
            {
                CityID = Guid.Parse("3377b955-7aad-46a1-8e42-a4564fd1c664"),
                CityName = "Jerusalem"
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityID = Guid.Parse("dd2a0f96-f9e9-40ec-9719-4d88d4bc1a23"),
                CityName = "Athens"
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityID = Guid.Parse("d4d68d61-2e25-40ee-9d62-49a71586248d"),
                CityName = "Rome"
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityID = Guid.Parse("bc9e17c6-a6cf-47a3-9c16-6cdfed114723"),
                CityName = "Beirut"
            });
        }

        #endregion
    }
}
