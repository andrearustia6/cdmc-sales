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
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<LeadCall> LeadCalls { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<ParticipantType> ParticipantTypes { get; set; }
        public DbSet<OnPhoneTemplate> OnPhoneTemplates { get; set; }
        public DbSet<OnPhoneBlockType> OnPhoneBlockTypes { get; set; }
        public DbSet<LeadCallType> LeadCallTypes { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<TargetOfMonth> TargetOfMonths { get; set; }
        public DbSet<TargetOfWeek> TargetOfWeeks { get; set; }
        public DbSet<DistrictNumber> DistrictNumbers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Department> Departments { get; set; }
        
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
            FakeData(context);
            context.SaveChanges();
        }

        private void FakeData(DB context)
        {
            context.Roles.Add(new Role() { Name = "高层", Level = 1000 });
            context.Roles.Add(new Role() { Name = "版块负责人", Level = 500 });
            context.Roles.Add(new Role() { Name = "销售经理", Level = 100 });
            context.Roles.Add(new Role() { Name = "销售专员", Level = 10 });
            context.Roles.Add(new Role() { Name = "产品部接口人", Level = 5 });
            context.Roles.Add(new Role() { Name = "市场部接口人", Level = 1 });

            context.CompanyTypes.Add(new CompanyType { Name = "外企独资企业",ID=1 });
            context.CompanyTypes.Add(new CompanyType { Name = "外企办事处",ID=2 });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业",ID=3 });
            context.CompanyTypes.Add(new CompanyType { Name = "国有企业",ID=4 });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业",ID=5 });
            context.CompanyTypes.Add(new CompanyType { Name = "民营企业",ID=6 });
            context.CompanyTypes.Add(new CompanyType { Name = "事业单位" ,ID=7});

         

            context.Categorys.Add(new Category (){ Name_EN="",Name_CH="航天",ID=1 });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "石油",ID=2 });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "化工",ID=3 });
            context.Categorys.Add(new Category() { Name_EN = "", Name_CH = "医疗",ID=4 });

            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "没有预算",Code=1 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无战略规划", Code = 2 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "请假", Code = 3 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无人接听", Code = 4 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "不是决策人", Code = 5 });

            context.LeadCallTypes.Add(new LeadCallType() { Name = "Others", Code = 1 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Blowed", Code = 2 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Not Pitched", Code = 3 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Pitched", Code = 4 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Full Pitched", Code = 5 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Call-Backed", Code = 6 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Waiting for Approval", Code = 7 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Qualified Decision", Code = 8 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Closed", Code = 9 });
            
             context.DistrictNumbers.Add(new DistrictNumber() { Country = "kuwait", Number = 965, TimeDifference = -5 ,ID=1});
            context.DistrictNumbers.Add(new DistrictNumber() { Country = "Australia", Number = 61, TimeDifference = 2  ,ID=2});

            context.CurrencyTypes.Add(new CurrencyType() { Name="RMB"});
            context.CurrencyTypes.Add(new CurrencyType() { Name = "$" });

            context.PaymentTypes.Add(new PaymentType() { Name = "现金" });
            context.PaymentTypes.Add(new PaymentType() { Name = "刷卡" });
            context.PaymentTypes.Add(new PaymentType() { Name = "汇款" });
            context.PaymentTypes.Add(new PaymentType() { Name = "其他" });

            context.PackageTypes.Add(new PackageType() {  Name_EN = "PLATINUM",ID=1 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "GLOD", ID = 2 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "SILVER", ID = 3 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "BRONZE", ID = 4 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "ASSOCIATE", ID = 5 });

            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Sponsor", ID = 1 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Delegate", ID = 2 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "VIP", ID = 3 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Others", ID = 4 });
            context.Packages.Add(new Package { Name_CH = "黄金展商", Name_EN = "30 minutes speechs", Prize = 20000, PackageTypeID=1, ParticipantTypeID=1,ID=1, SubName="" });

            context.Companys.Add(new Company() { CategoryID = 1, CompanyTypeID = 1, Cerator = "karen", Areas = "航天上下游相关企业", Contact = "210000000", DistrictNumberID = 1, From="销售部", ID=1, Name_CH="天空之星", Name_EN="Sky Start",  Fax="213000" });

            context.Leads.Add(new Lead() {  ID = 1, Name_CH = "马克", Name_EN = "Mike", Fax = "213000", CompanyID=1, Contact="23422232", Mobile="1243323233", Title="CEO", Gender="Mr", EMail="Mike@123.com"  });

            context.Projects.Add(new Project() { ID = 1, EndDate=DateTime.Now, StartDate=DateTime.Now, Target=50000000, IsActived=true, Leader="susie", Manager="john", Name="CTC 航空峰会" });
            context.Projects.Add(new Project() { ID = 2, EndDate = DateTime.Now, StartDate = DateTime.Now, Target = 30000000, IsActived = true, Leader = "susie", Manager = "john", Name = "万国博览会" });

            context.Members.Add(new Member() { ID = 1, ProjectID=1, Name="Mike"  });
            context.Members.Add(new Member() { ID = 2, ProjectID = 1, Name = "susie" });
            context.Members.Add(new Member() { ID = 3, ProjectID = 1, Name = "flora" });

            context.Members.Add(new Member() { ID = 4, ProjectID = 2, Name = "Mike" });
            context.Members.Add(new Member() { ID = 5, ProjectID = 2, Name = "susie" });
            context.Members.Add(new Member() { ID = 6, ProjectID = 2, Name = "flora" });
        }
    }


}