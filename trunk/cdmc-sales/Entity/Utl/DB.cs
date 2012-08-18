using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Utl
{
    public class DB : DbContext
    {
        public DbSet<CurrencyType> LeadtTypes { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<Lead> Leads { get; set; }
        
        public DbSet<Category> Categorys { get; set; }
        //public DbSet<LeadSheet> LeadSheets { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DB()
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DbContext>()); 
            Database.SetInitializer<DB>(new DBInitializer());
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           

            //modelBuilder.Conventions..Remove<>();
            // Ensure that IncludeMetadataConvention has been added to the DbModelBuilder conventions.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class DBInitializer : DropCreateDatabaseIfModelChanges<DB>
    {
        protected override void Seed(DB context)
        {
            FackData();
            context.SaveChanges();
        }

        private void FackData()
        {

        }
    }

   
}