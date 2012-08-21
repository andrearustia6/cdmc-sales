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
        public DbSet<RoleLevel> RoleLevels { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<LeadSheet> LeadSheets { get; set; }
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
            FackData(context);
            context.SaveChanges();
        }

        private void FackData(DB context)
        {
            context.RoleLevels.Add(new RoleLevel(){ Name="高层",Level=1000});
            context.RoleLevels.Add(new RoleLevel() { Name = "版块负责人", Level = 500 });
            context.RoleLevels.Add(new RoleLevel() { Name = "销售经理", Level = 100 });
            context.RoleLevels.Add(new RoleLevel() { Name = "销售专员", Level = 1 });
        }
    }

   
}