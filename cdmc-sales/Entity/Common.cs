﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class AssignPerformanceScore:EntityBase{
        [Display(Name = "考核系数"), Required]
        public double? Rate { get; set; }
        [Display(Name = "主观打分"), Required]
        public int? Score { get; set; }
        [Display(Name = "被考核人"), Required]
        public string TargetName { get; set; }
        [Display(Name = "主管"), Required]
        public string Assigner { get; set; }
        [Display(Name = "考核系数设置者"), Required]
        public string RateAssigner { get; set; }
        [Display(Name = "月"), Required]
        public  int? Month{get;set;}
        [Display(Name = "年"), Required]
        public int? Year { get; set; }
    }

    public class AssignPerformanceRate : EntityBase
    {
        [Display(Name = "考核系数"), Required]
        public double Rate { get; set; }
        [Display(Name = "被考核人"), Required]
        public string TargetName { get; set; }
        [Display(Name = "月"), Required]
        public int Month { get; set; }
        [Display(Name = "年"), Required]
        public int Year { get; set; }
    }

    /// <summary>
    /// 行业类型
    /// </summary>
    public class Area : NameEntity
    {
    }

    public class AccessRight : EntityBase
    {
        [Display(Name = "权限名称"), Required]
        public string Name { get; set; }

        [Display(Name = "权限编码")]
        public int? Code { get; set; }

        [Display(Name = "权限类型"), Required]
        public string RightType { get; set; }

        [Display(Name = "访问链接")]
        public string AccessRightDetails { get; set; }


        public virtual List<ProjectRight> ProjectRights { get; set; } 
    }

    public class ProjectRight : EntityBase
    {
        public virtual Project Project { get; set; }
        [Display(Name = "所属项目")]
        public int? ProjectID { get; set; }
        [Display(Name = "用户姓名")]
        public string Name { get; set; }
        public string AccessRightString 
        { get
        { 
            string result = string.Empty;
            if (AccessRights == null) return result;

            AccessRights.ForEach(f => {
                if (result == string.Empty)
                    result += f.Name;
                else
                    result +=  " | " + f.Name;
            });
            return result;
            
        } }
        public virtual List<AccessRight> AccessRights { get; set; }
    }

    /// <summary>
    /// 新闻
    /// </summary>
    public class News : EntityBase
    {
        [Display(Name = "新闻标题"), Required]
        public string Name { get; set; }

        [Display(Name = "新闻类型")]
        public int? Code { get; set; }

        [Display(Name = "新闻副标题")]
        public string SubName { get; set; }

        [Display(Name = "新闻内容"), Required]
        public string Content { get; set; }

        [Display(Name = "新闻链接")]
        public string Link { get; set; }

        public virtual Project Project { get; set; }
        public int? ProjectID { get; set; }

    }

    /// <summary>
    /// 常见问题
    /// </summary>
    public class FAQ : EntityBase
    {
        [Display(Name = "问题标题"), StringLength(200), Required]
        public String Name { get; set; }
        [Display(Name = "问题类型"), Required]
        public int Code { get; set; }
        [Display(Name = "副标题"), StringLength(200)]
        public string SubName { get; set; }

        [Display(Name = "问题描述"), Required]
        public string Question { get; set; }

        [Display(Name = "问题答案"), Required]
        public string Answer { get; set; }
    }


    /// <summary>
    /// 区号
    /// </summary>
    public class DistrictNumber : EntityBase
    {
         [Display(Name = "时差")]
        public string Name { get { return Country + ":" + Number.ToString() + "(" + TimeDifference.ToString() + ")"; } }
         
        [Display(Name = "区号"),  Required]
        public int Number { get; set; }

        [Display(Name = "国家"), Required]
        public string Country { get; set; }

        [Display(Name = "时差"), Required]
        public double TimeDifference { get; set; }
    }

    /// <summary>
    /// 留言
    /// </summary>
    public class Message : EntityBase
    {

        [Display(Name = "问题单号")]
        public String FlowNumber { get; set; }

        [Display(Name = "求助人")]
        public  string Member { get; set; }
        
        public virtual Project Project { get; set; }
        [Display(Name = "所属项目"), Required]
        public int? ProjectID { get; set; }

        [Display(Name = "解决人")]
        public String Solver { get; set; }

        [Display(Name = "问题标题"), MaxLength(200), Required]
        public String Question { get; set; }


        [Display(Name = "提问内容"), Required]
        public String Content { get; set; }

        [Display(Name = "解决方案")]
        public String Answer { get; set; }

        [Display(Name = "已解答")]
        public bool IsAnswered { get; set; }

        
        public virtual SalesType SalesType { get; set; }
        [Display(Name = "提问角色")]
        public int? SalesTypeID { get; set; }

    }


    /// <summary>
    /// 关键字
    /// </summary>
    public class Keyword : EntityBase
    {
        [Display(Name = "名称"), Required]
        public string Name { get; set; }

        [Display(Name = "关键字解释")]
        public string Explanation { get; set; }
    }

    /// <summary>
    /// 货币类型
    /// </summary>
    public class CurrencyType : EntityBase
    {
        [Display(Name = "货币类型")]
        public string Name { get; set; }

        public Double Rate { get; set; }
    }
    
      /// <summary>
    /// 付款类型
    /// </summary>
    public class PaymentType : EntityBase
    {
        [Display(Name = "付款类型")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 图片
    /// </summary>
    public class Image : EntityBase
    {
        [Column(TypeName = "image")]
        public Byte[] ImageData { get; set; }

        [Display(Name = "文件类型")]
        public string ContentType { get; set; }

        [Display(Name = "图片名称")]
        public string Name { get; set; }

        [Display(Name = "应用类型")]
        public string ImageArea { get; set; }
    }

    public enum ImageArea { LoginPage}
}
