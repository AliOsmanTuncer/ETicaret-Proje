using Eticaret.Core.Entities;
using Eticaret.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Eticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //LOKAL SUNUCU
            //optionsBuilder.UseSqlServer(@"Server=MSI-ALI\INSTANCE2024; Database=EticaretDb; Integrated Security=True; TrustServerCertificate=True;");
            //MONSTERASP.NET
            //optionsBuilder.UseSqlServer(@"Server=db14455.databaseasp.net; Database=db14455; User Id=db14455; Password=dD?52rE-e_9B; Encrypt=False; MultipleActiveResultSets=True;");
            optionsBuilder.UseSqlServer(@"Server=db14455.public.databaseasp.net; 
                              Database=db14455; 
                              User Id=db14455; 
                              Password=dD?52rE-e_9B; 
                              Encrypt=False; 
                              MultipleActiveResultSets=True;");


            //Bu satırı kaldırabilirim. Migration ve update database komutu çalıştırken hata almamak için 
            //optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Teker teker config 
            //modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            //modelBuilder.ApplyConfiguration(new BrandConfiguration());
            //modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            //modelBuilder.ApplyConfiguration(new ContactConfiguration());
            //modelBuilder.ApplyConfiguration(new NewsConfiguration());
            //modelBuilder.ApplyConfiguration(new ProductConfiguration());
            //modelBuilder.ApplyConfiguration(new SliderConfiguration());

            //Classların çalıştığı yeri bulup otomatik aynı işlemi yapar.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
