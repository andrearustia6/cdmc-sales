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
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<LeadSheet> LeadSheets { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<ParticipantType> ParticipantTypes { get; set; }
        public DbSet<OnPhoneTemplate> OnPhoneTemplates { get; set; }
        public DbSet<OnPhoneBlockType> OnPhoneBlockTypes { get; set; }
        public DbSet<LeadSheetType> LeadSheetTypes { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }
        public DbSet<PackageServiceType> PackageServiceTypes { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<DistrictNumber> DistrictNumbers { get; set; }
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
            context.Roles.Add(new Role() { Name = "高层", Level = 1000 });
            context.Roles.Add(new Role() { Name = "版块负责人", Level = 500 });
            context.Roles.Add(new Role() { Name = "销售经理", Level = 100 });
            context.Roles.Add(new Role() { Name = "销售专员", Level = 10 });
            context.Roles.Add(new Role() { Name = "产品部接口人", Level = 5 });
            context.Roles.Add(new Role() { Name = "市场部接口人", Level = 1 });

            context.CompanyTypes.Add(new CompanyType { Name = "外企独资企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "外企办事处" });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "国有企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "民营企业" });
            context.CompanyTypes.Add(new CompanyType { Name = "事业单位" });

            context.Titles.Add(new Title() { Name_EN = "CEO",Name_CH="首席执行官" });
            context.Titles.Add(new Title() { Name_EN = "CFO",Name_CH="财务总监"});
            context.Titles.Add(new Title() { Name_EN = "CTO",Name_CH="技术总监" });
            context.Titles.Add(new Title() { Name_EN = "MarketDirector", Name_CH="市场总监" });

            context.Categorys.Add(new Category (){ Name_EN="",Name_CH="航天" });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "石油" });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "化工" });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "医疗" });

            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "没有预算",Code=1 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无战略规划", Code = 2 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "请假", Code = 3 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无人接听", Code = 4 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "不是决策人", Code = 5 });

            context.LeadSheetTypes.Add(new LeadSheetType() { Name="Others", Code=1});
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Not Pitched", Code = 2 });
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Waiting for Approval", Code = 3 });
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Closed", Code = 4 });
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Blowed", Code = 5 });
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Pitched", Code = 6 });
            context.LeadSheetTypes.Add(new LeadSheetType() { Name = "Call-Backed", Code = 7 });

            context.CurrencyTypes.Add(new CurrencyType() { Name="RMB"});
            context.CurrencyTypes.Add(new CurrencyType() { Name = "$" });

            context.PaymentTypes.Add(new PaymentType() { Name = "现金" });
            context.PaymentTypes.Add(new PaymentType() { Name = "刷卡" });
            context.PaymentTypes.Add(new PaymentType() { Name = "汇款" });
            context.PaymentTypes.Add(new PaymentType() { Name = "其他" });

            context.PackageTypes.Add(new PackageType() { Name = "PLATINUM" });
            context.PackageTypes.Add(new PackageType() { Name = "GLOD" });
            context.PackageTypes.Add(new PackageType() { Name = "SILVER" });
            context.PackageTypes.Add(new PackageType() { Name = "BRONZE" });
            context.PackageTypes.Add(new PackageType() { Name = "ASSOCIATE" });

            context.ParticipantTypes.Add(new ParticipantType() { Name = "Sponsor" });
            context.ParticipantTypes.Add(new ParticipantType() {  Name="Delegate"});
            context.ParticipantTypes.Add(new ParticipantType() { Name = "VIP" });
            context.ParticipantTypes.Add(new ParticipantType() { Name = "Others" });

            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Opening Remark" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Keynote Address" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Pre-scheduled One-to-one Meetings" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Featured Speech" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Chairperson" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Panel Discussion" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "VIP Reception" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "On Site Logo Exposure" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Exhibit Space" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Insertion" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Complimentary Sponsorship" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Press Interview" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Free delegate passes" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Additional registration discount" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Wishing Clients Invitation" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Media Exposure" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Event Website Advertising" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Conference Facsimile Promotions" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Conference Email Promotion" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Color Glossy Summit Brochures" });
            context.PackageServiceTypes.Add(new PackageServiceType() { Name = "Value-added Services" });

            
            context.PackageItems.Add(new PackageItem() { Name="两天三夜", Content="提供三天晚上的五星级酒店标准住宿", PackageServiceTypeID=1, PackageTypeID=1, ParticipantTypeID=1 });
            context.PackageItems.Add(new PackageItem() { Name="15分钟演讲", Content="15分钟大会演讲", PackageServiceTypeID=2, PackageTypeID=1, ParticipantTypeID=1 });

            var data = context.PackageItems.ToList();
            var pakage = new Package() { Name = "Sponsor Package", ParticipantTypeID = 1 };
            pakage.PackageItems.AddRange(data);
            context.Packages.Add(pakage);

            context.DistrictNumbers.Add(new DistrictNumber() { Country = "kuwait", Number = 965, TimeDifference = -5 });
            context.DistrictNumbers.Add(new DistrictNumber() { Country = "Australia", Number = 61, TimeDifference = 2 });
        }
    }


}