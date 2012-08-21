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

        public DbSet<Research> Researches { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<FAQ> FAQs { get; set; }

        public DbSet<CurrencyType> CurrencyTypes { get; set; }

        public DbSet<Title> Title { get; set; }

        public DbSet<PaymentType> PaymentType { get; set; }

        public DbSet<ParticipantType> ParticipantType { get; set; }

        public DbSet<PackageServiceType> PackageServiceType { get; set; }

        public DbSet<PackageType> PackageType { get; set; }

        public DbSet<LeadSheetType> LeadSheetType { get; set; }

        public DbSet<TemplateType> TemplateType { get; set; }
    }
}
