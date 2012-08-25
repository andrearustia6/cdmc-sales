using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;

namespace Utl
{
    public class DB : DbContext
    {
        public DbSet<CurrencyType> LeadtTypes { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Role> Roles { get; set; }
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
            context.Roles.Add(new Role(){ Name="高层",Level=1000});
            context.Roles.Add(new Role() { Name = "版块负责人", Level = 500 });
            context.Roles.Add(new Role() { Name = "销售经理", Level = 100 });
            context.Roles.Add(new Role() { Name = "销售专员", Level = 10 });
            context.Roles.Add(new Role() { Name = "产品部接口人", Level = 5 });
            context.Roles.Add(new Role() { Name = "市场部接口人", Level = 1 });

            context.CompanyTypes.Add(new CompanyType { Name = "外企独资企业"});
            context.CompanyTypes.Add(new CompanyType { Name = "外企办事处" });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "国有企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "民营企业" });
        }
    }

   
}