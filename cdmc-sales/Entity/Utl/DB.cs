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
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<Category> Areas { get; set; }
        public DbSet<CallingResult> CallRecords { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DB()
        {
            Database.SetInitializer<DB>(new DBInitializer());
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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