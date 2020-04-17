using EmpoolysMangment.Models;
using EmpoolysMangment.ViewModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpoolysMangment.Data
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          
           /* foreach(var forinKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())){
                forinKey.DeleteBehavior = DeleteBehavior.Restrict;
            }*/

        }
        public DbSet<Employee> Employees { get; set; }
        


    }
}
