using BouvetWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BouvetWebApp.Data
{
    public class OrgContext : DbContext
    {

        public OrgContext(DbContextOptions<OrgContext> options) 
            : base(options)
        {
        }
        
        public DbSet<Enheter> Enheter { get; set; }
        
        public DbSet<Organisasjonsform> Organisasjonsform { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enheter>().Property(x => x.Organisasjonsnummer).ValueGeneratedNever();
            modelBuilder.Entity<Enheter>().ToTable("Companies");
            
            modelBuilder.Entity<Organisasjonsform>().ToTable("OrganizationType");
        }
    }
}