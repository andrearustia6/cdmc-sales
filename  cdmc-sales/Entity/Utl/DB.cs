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
        public DbSet<Client> Clients { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<CallingRecord> CallRecords { get; set; }

        public DB()
        {
            Database.SetInitializer<DB>(new WorkflowInitializer());
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class WorkflowInitializer : DropCreateDatabaseIfModelChanges<DB>
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