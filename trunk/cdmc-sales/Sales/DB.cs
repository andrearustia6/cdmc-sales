using System.Data.Entity;
using Entity;

namespace Sales
{
    public class DB : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Sales.DB>());

        public DbSet<LeadCall> LeadCall { get; set; }

        public DbSet<Lead> Lead { get; set; }

        public DbSet<CompanyRelationship> CompanyRelationship { get; set; }

        public DbSet<LeadCallType> LeadCallType { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Company> Company { get; set; }

        public DbSet<Progress> Progress { get; set; }

        public DbSet<Template> Template { get; set; }

        public DbSet<TemplateType> TemplateType { get; set; }

        public DbSet<SalesType> SalesType { get; set; }

        public DbSet<Message> Message { get; set; }
    }
}
