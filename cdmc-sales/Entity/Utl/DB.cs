﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using EntityUtl;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects;


namespace Utl
{
    public class DB : DbContext
    {
        public DbSet<CrmTrack> CrmTracks { get; set; }
        public DbSet<CrmCommentState> CrmCommentStates { get; set; } 
        public DbSet<Progress> Progresss { get; set; }
        public DbSet<ProgressTrack> ProgressTrack { get; set; }
        public DbSet<CoreLVL> CoreLVLs { get; set; }
        public DbSet<AccessRight> AccessRights { get; set; }
        public DbSet<ProjectRight> ProjectRights { get; set; }
        public DbSet<AssignPerformanceRate> AssignPerformanceRates { get; set; }
        public DbSet<AssignPerformanceScore> AssignPerformanceScores { get; set; }
        public DbSet<ExpLevel> ExpLevels { get; set; }
        public DbSet<SalesType> SalesTypes { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<LeadCall> LeadCalls { get; set; }
        public DbSet<Research> Researchs { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<ParticipantType> ParticipantTypes { get; set; }
        public DbSet<PhoneSaleSupport> OnPhoneTemplates { get; set; }
        public DbSet<OnPhoneBlockType> OnPhoneBlockTypes { get; set; }
        public DbSet<LeadCallType> LeadCallTypes { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<SimulatorConfig> SimulatorConfigs { get; set; }
        public DbSet<TargetOfMonth> TargetOfMonths { get; set; }
        public DbSet<TargetOfWeek> TargetOfWeeks { get; set; }
        public DbSet<DistrictNumber> DistrictNumbers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectReview> ProjectReviews { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<CompanyRelationship> CompanyRelationships { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<SubCompany> SubCompanys { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TargetOfMonthForMember> TargetOfMonthForMembers { get; set; }

        public DbSet<UserFavorsCrmGroup> UserFavorsCrmGroups { get; set; }
        public DbSet<UserFavorsCRM> UserFavorsCRMs { get; set; }

        public DbSet<ImportCompanyTrace> ImportCompanyTrace { get; set; }
        public DbSet<ManagerScore> ManagerScores { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<PreCommission> PreCommissions { get; set; }
        public DbSet<FinalCommission> FinalCommissions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentContent> CommentContents { get; set; }
        public DbSet<CompanyMergeTrack> CompanyMergeTracks { get; set; }
        public DB(string connection = null)
        {

            //Database.SetInitializer<DB>(new DBInitializer());
            Database.SetInitializer<DB>(null);
            if (!string.IsNullOrEmpty(connection))
                this.Database.Connection.ConnectionString = connection;

            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public void Detach(object entity)
        {
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }



    }

    public class DBInitializer : DropCreateDatabaseIfModelChanges<DB>
    {
        protected override void Seed(DB context)
        {
            //FakeData(context);
            //context.SaveChanges();
        }

        private void FakeData(DB context)
        {
            context.Roles.Add(new Role() { Name = "高层", Level = 1000, ID = 1 });
            context.Roles.Add(new Role() { Name = "版块负责人", Level = 500, ID = 2 });
            context.Roles.Add(new Role() { Name = "销售经理", Level = 100, ID = 3 });
            context.Roles.Add(new Role() { Name = "销售专员", Level = 10, ID = 4 });
            context.Roles.Add(new Role() { Name = "产品部接口人", Level = 5, ID = 5 });
            context.Roles.Add(new Role() { Name = "会务部部接口人", Level = 3, ID = 6 });
            context.Roles.Add(new Role() { Name = "市场部接口人", Level = 1, ID = 7 });

            context.CompanyTypes.Add(new CompanyType { Name = "外企独资企业", ID = 1 });
            context.CompanyTypes.Add(new CompanyType { Name = "外企办事处", ID = 2 });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业", ID = 3 });
            context.CompanyTypes.Add(new CompanyType { Name = "国有企业", ID = 4 });
            context.CompanyTypes.Add(new CompanyType { Name = "合资企业", ID = 5 });
            context.CompanyTypes.Add(new CompanyType { Name = "民营企业", ID = 6 });
            context.CompanyTypes.Add(new CompanyType { Name = "事业单位", ID = 7 });



            context.Areas.Add(new Area() { Name_EN = "", Name_CH = "航天", ID = 1 });
            context.Areas.Add(new Area() { Name_EN = "", Name_CH = "石油", ID = 2 });
            context.Areas.Add(new Area() { Name_EN = "", Name_CH = "化工", ID = 3 });
            context.Areas.Add(new Area() { Name_EN = "", Name_CH = "医疗", ID = 4 });
            context.Areas.Add(new Area() { Name_EN = "Publishing", Name_CH = "出版行业", ID = 5 });

            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "没有预算", Code = 1 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无战略规划", Code = 2 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "请假", Code = 3 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "无人接听", Code = 4 });
            context.OnPhoneBlockTypes.Add(new OnPhoneBlockType() { Name = "不是决策人", Code = 5 });

            context.LeadCallTypes.Add(new LeadCallType() { Name = "Others", Code = 10, ID = 1 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Blowed", Code = 20, ID = 2 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Not Pitched", Code = 30, ID = 3 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Pitched", Code = 40, ID = 4 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Full Pitched", Code = 50, ID = 5 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Call-Backed", Code = 60, ID = 6 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Waiting for Approval", Code = 70, ID = 7 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Qualified Decision", Code = 80, ID = 8 });
            context.LeadCallTypes.Add(new LeadCallType() { Name = "Closed", Code = 90, ID = 9 });

            context.DistrictNumbers.Add(new DistrictNumber() { Country = "kuwait", Number = 965, TimeDifference = -5, ID = 1 });
            context.DistrictNumbers.Add(new DistrictNumber() { Country = "Australia", Number = 61, TimeDifference = 2, ID = 2 });
            context.DistrictNumbers.Add(new DistrictNumber() { Country = "India", Number = 91, TimeDifference = -2.3, ID = 3 });



            context.CurrencyTypes.Add(new CurrencyType() { Name = "RMB", ID = 1 });
            context.CurrencyTypes.Add(new CurrencyType() { Name = "$", ID = 2 });

            context.SalesTypes.Add(new SalesType() { Name = "基础销售", ID = 1 });
            context.SalesTypes.Add(new SalesType() { Name = "客户经理", ID = 2 });

            context.PaymentTypes.Add(new PaymentType() { Name = "现金" });
            context.PaymentTypes.Add(new PaymentType() { Name = "刷卡" });
            context.PaymentTypes.Add(new PaymentType() { Name = "汇款" });
            context.PaymentTypes.Add(new PaymentType() { Name = "其他" });

            context.PackageTypes.Add(new PackageType() { Name_EN = "PLATINUM", ID = 1 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "GLOD", ID = 2 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "SILVER", ID = 3 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "BRONZE", ID = 4 });
            context.PackageTypes.Add(new PackageType() { Name_EN = "ASSOCIATE", ID = 5 });

            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Sponsor", ID = 1 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Delegate", ID = 2 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "VIP", ID = 3 });
            context.ParticipantTypes.Add(new ParticipantType() { Name_EN = "Others", ID = 4 });

            context.Packages.Add(new Package { Name_CH = "黄金展商", Name_EN = "30 minutes speechs", Prize = 20000, PackageTypeID = 1, ParticipantTypeID = 1, ID = 1, SubName = "" });

            var c1 = new Company() { AreaID = 1, CompanyTypeID = 1, Cerator = "sean", Creator = "karen", Contact = "2100000", DistrictNumberID = 1, From = "销售部", ID = 1, Name_CH = "天空之星", Name_EN = "Sky Start", Fax = "213000" };
            context.Companys.Add(c1);
            var c2 = new Company() { AreaID = 2, CompanyTypeID = 1, Cerator = "sean", Contact = "214545400", Creator = "karen", DistrictNumberID = 1, From = "销售部", ID = 2, Name_CH = "火箭之家", Name_EN = "Rocket Me", Fax = "214500" };
            context.Companys.Add(c2);
            var c3 = new Company() { AreaID = 3, CompanyTypeID = 1, Cerator = "susie", Contact = "21566500", Creator = "karen", DistrictNumberID = 1, From = "销售部", ID = 3, Name_CH = "高精行空油业", Name_EN = "Elite Sky Oil", Fax = "563000" };
            context.Companys.Add(c3);

            var l1 = new Lead() { ID = 1, Name_CH = "马克", Name_EN = "Mike", Fax = "213000", CompanyID = 1, Contact = "23422232", Mobile = "1243323233", Title = "CEO", Gender = "Mr", EMail = "Mike@123.com" };
            context.Leads.Add(l1);

            var l2 = new Lead() { ID = 2, Name_CH = "艾利克斯", Name_EN = "Alex", Fax = "214500", CompanyID = 1, Contact = "23422232", Mobile = "1248903233", Title = "CFO", Gender = "Mr", EMail = "Alex@123.com" };
            context.Leads.Add(l2);
            var l3 = new Lead() { ID = 3, Name_CH = "马丁", Name_EN = "Martin", Fax = "254000", CompanyID = 2, Contact = "2342752", Mobile = "1343323233", Title = "Sales Director", Gender = "Mr", EMail = "Martin@123.com" };
            context.Leads.Add(l3);

            var l4 = new Lead() { ID = 4, Name_CH = "汤玛斯", Name_EN = "Tomas", Fax = "563000", CompanyID = 3, Contact = "23674232", Mobile = "1256423233", Title = "CTO", Gender = "Mr", EMail = "Tomas@123.com" };
            context.Leads.Add(l4);

            var p1 = new Project()
            {
                ID = 1,
                EndDate = DateTime.Now.AddYears(1),
                ConferenceStartDate = new DateTime(2012, 12, 6),
                ConferenceEndDate = new DateTime(2012, 12, 7),
                StartDate = DateTime.Now.AddYears(-1),
                Target = 50000000,
                IsActived = true,
                TeamLeader = "sean",
                Manager = "mike",
                Name_CH = "CTC 航空峰会",
                ProjectCode = "ACYY",
                Market = "amy",
                Product = "flora"
            };
            p1.CompanyRelationships = new List<CompanyRelationship>();
            var cr1 = new CompanyRelationship() { ID = 1, ProjectID = 1, CompanyID = 1, Importancy = 6 };
            var cr2 = new CompanyRelationship() { ID = 2, ProjectID = 1, CompanyID = 2, Importancy = 3 };
            context.CompanyRelationships.Add(cr1);
            context.CompanyRelationships.Add(cr2);

            p1.CompanyRelationships.Add(cr1);
            p1.CompanyRelationships.Add(cr2);

            var tm1 = new TargetOfMonth() { ID = 1, BaseDeal = 50000, CheckIn = 50000, Deal = 70000, StartDate = DateTime.Now.StartOfMonth(), EndDate = DateTime.Now.EndOfMonth() };
            context.TargetOfMonths.Add(tm1);
            p1.TargetOfMonths = new List<TargetOfMonth>();
            p1.TargetOfMonths.Add(tm1);

            var tw1 = new TargetOfWeek() { ID = 1, Member = "sean", TargetOfMonthID = 1, ProjectID = 1, CheckIn = 10000, Deal = 12000, EndDate = DateTime.Now.EndOfWeek(), StartDate = DateTime.Now.StartOfWeek() };
            context.TargetOfWeeks.Add(tw1);

            var tw2 = new TargetOfWeek() { ID = 2, Member = "sean", TargetOfMonthID = 1, ProjectID = 1, CheckIn = 10300, Deal = 15000, EndDate = DateTime.Now.EndOfWeek().AddDays(7), StartDate = DateTime.Now.StartOfWeek().AddDays(7) };
            context.TargetOfWeeks.Add(tw2);

            var tw3 = new TargetOfWeek() { ID = 3, Member = "susie", TargetOfMonthID = 1, ProjectID = 1, CheckIn = 8500, Deal = 11000, EndDate = DateTime.Now.EndOfWeek(), StartDate = DateTime.Now.StartOfWeek() };
            context.TargetOfWeeks.Add(tw3);

            var tw4 = new TargetOfWeek() { ID = 4, Member = "tina", TargetOfMonthID = 1, ProjectID = 1, CheckIn = 7000, Deal = 6000, EndDate = DateTime.Now.EndOfWeek(), StartDate = DateTime.Now.StartOfWeek() };
            context.TargetOfWeeks.Add(tw4);

            var d1 = new Deal() { ExpectedPaymentDate = DateTime.Now.AddDays(-2), SignDate = DateTime.Now.AddDays(-5), ActualPaymentDate = DateTime.Now, ID = 1, Income = 12000, Payment = 12000, PackageID = 1, CompanyRelationshipID = 1, Sales = "sean", IsClosed = true };
            var d2 = new Deal() { ExpectedPaymentDate = DateTime.Now.AddDays(-1), SignDate = DateTime.Now.AddDays(-3), ActualPaymentDate = DateTime.Now, ID = 2, Income = 5000, Payment = 5000, PackageID = 1, CompanyRelationshipID = 1, Sales = "tina", IsClosed = true };
            var d4 = new Deal() { ExpectedPaymentDate = DateTime.Now.AddDays(-3), SignDate = DateTime.Now.AddDays(-6), ActualPaymentDate = DateTime.Now, ID = 4, Income = 7500, Payment = 7500, PackageID = 1, CompanyRelationshipID = 2, Sales = "sean", IsClosed = true };
            var d3 = new Deal() { ExpectedPaymentDate = DateTime.Now.AddDays(7), SignDate = DateTime.Now.AddDays(-2), ID = 3, Income = 0, Payment = 60000, PackageID = 1, CompanyRelationshipID = 1, Sales = "sean", IsClosed = false };

            context.Deals.Add(d1);
            context.Deals.Add(d2);
            context.Deals.Add(d3);
            context.Deals.Add(d4);

            var lc1 = new LeadCall() { ID = 1, CreatedDate = DateTime.Now.AddDays(-5), CallDate = DateTime.Now.AddDays(-5), Creator = "sean", MemberID = 1, LeadCallTypeID = 3, LeadID = 1, CompanyRelationshipID = 1 };
            var lc2 = new LeadCall() { ID = 2, CreatedDate = DateTime.Now.AddDays(-4), CallDate = DateTime.Now.AddDays(-4), Creator = "sean", MemberID = 1, LeadCallTypeID = 4, LeadID = 1, CompanyRelationshipID = 1 };
            var lc3 = new LeadCall() { ID = 3, CreatedDate = DateTime.Now.AddDays(-3), CallDate = DateTime.Now.AddDays(-4), Creator = "sean", MemberID = 1, LeadCallTypeID = 5, LeadID = 1, CompanyRelationshipID = 1 };
            var lc5 = new LeadCall() { ID = 4, CreatedDate = DateTime.Now.AddDays(-2), CallDate = DateTime.Now.AddDays(-4), Creator = "sean", MemberID = 1, LeadCallTypeID = 6, CallBackDate = DateTime.Now.AddDays(1), LeadID = 2, CompanyRelationshipID = 1 };
            var lc6 = new LeadCall() { ID = 5, CreatedDate = DateTime.Now.AddDays(-1), CallDate = DateTime.Now.AddDays(-4), Creator = "sean", MemberID = 1, LeadCallTypeID = 6, CallBackDate = DateTime.Now.AddDays(2), LeadID = 1, CompanyRelationshipID = 1 };
            var lc7 = new LeadCall() { ID = 6, CreatedDate = DateTime.Now.AddDays(-3), CallDate = DateTime.Now.AddDays(-4), Creator = "susie", MemberID = 3, LeadCallTypeID = 2, LeadID = 3, CompanyRelationshipID = 2 };
            var lc4 = new LeadCall() { ID = 7, CreatedDate = DateTime.Now.AddDays(-1), CallDate = DateTime.Now.AddDays(-4), Creator = "susie", MemberID = 3, LeadCallTypeID = 2, LeadID = 3, CompanyRelationshipID = 2 };

            context.LeadCalls.Add(lc1);
            context.LeadCalls.Add(lc2);
            context.LeadCalls.Add(lc3);
            context.LeadCalls.Add(lc4);
            context.LeadCalls.Add(lc5);
            context.LeadCalls.Add(lc6);
            context.LeadCalls.Add(lc7);

            context.Projects.Add(p1);
            var p2 = new Project()
            {
                ID = 2,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                ConferenceStartDate = new DateTime(2012, 12, 6),
                ConferenceEndDate = new DateTime(2012, 12, 7),
                Target = 30000000,
                IsActived = true,
                TeamLeader = "stone",
                Manager = "mike",
                Name_CH = "万国博览会",
                ProjectCode = "BTEC"
            };
            context.Projects.Add(p2);

            var p3 = new Project()
            {
                ID = 3,
                EndDate = new DateTime(2012, 12, 6),
                StartDate = new DateTime(2012, 7, 16),
                ConferenceStartDate = new DateTime(2012, 12, 6),
                ConferenceEndDate = new DateTime(2012, 12, 7),
                Target = 900000,
                IsActived = true,
                TeamLeader = "stone",
                Manager = "mike",
                Name_CH = "2012亚洲数字出版高峰论坛暨颁奖盛典",
                Name_EN = "Asia Digital Publishing Summit & Awards 2012",
                ProjectCode = "ADPSA2012",


            };
            context.Projects.Add(p3);


            context.Members.Add(new Member() { ID = 1, ProjectID = 1, Name = "sean", Characters = "R|S", SalesTypeID = 2 });
            context.Members.Add(new Member() { ID = 2, ProjectID = 1, Name = "susie", SalesTypeID = 1 });
            context.Members.Add(new Member() { ID = 3, ProjectID = 1, Name = "tina", SalesTypeID = 1 });

            context.Members.Add(new Member() { ID = 4, ProjectID = 2, Name = "stone", SalesTypeID = 2 });
            context.Members.Add(new Member() { ID = 5, ProjectID = 2, Name = "john", SalesTypeID = 1 });
            context.Members.Add(new Member() { ID = 6, ProjectID = 2, Name = "lucas", SalesTypeID = 1 });
            context.Members.Add(new Member() { ID = 7, ProjectID = 2, Name = "tina", SalesTypeID = 1 });
            context.Members.Add(new Member() { ID = 8, ProjectID = 3, Name = "rachel", SalesTypeID = 1 });
        }
    }

    public class Bill
    {

        public string Phone { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
        public string DialNumber { get; set; }
    }

    public class BillDB : DbContext
    {


        public DbSet<Bill> Bills { get; set; }


        public BillDB()
        {
            //Database.SetInitializer<DB>(new DBInitializer());
            Database.SetInitializer<BillDB>(null);



            this.Configuration.ValidateOnSaveEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }


    }


    public class PDDB : DbContext
    {
        public DbSet<Organization> Organizations { set; get; }
        public DbSet<Speaker> Speakers { set; get; }
        public DbSet<Conference> Conferences { set; get; }
        public DbSet<ClientDurationType> ClientDurationTypes { set; get; }
        public PDDB(string connection = null)
        {
            Database.SetInitializer<PDDB>(null);
            if (!string.IsNullOrEmpty(connection))
                this.Database.Connection.ConnectionString = connection;

            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

       
        public void Detach(object entity)
        {
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }
    }
}