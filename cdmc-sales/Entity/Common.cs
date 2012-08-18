using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    /// <summary>
    /// 行业类型
    /// </summary>
    public class Category : EntityBase
    {
        [Display(Name = "行业名称"), Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// 新闻
    /// </summary>
    public class News : EntityBase
    {
        [Display(Name = "新闻标题"), Required]
        public string Name { get; set; }

        [Display(Name = "新闻类型"), Required]
        public int Code { get; set; }

        [Display(Name = "新闻副标题")]
        public string SubName { get; set; }

        [Display(Name = "新闻内容"), Required]
        public string Content { get; set; }

        [Display(Name = "新闻链接"), Required]
        public string Link { get; set; }
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
    /// 留言
    /// </summary>
    public class Message : EntityBase
    {
        [Display(Name = "问题标题"), MaxLength(200), Required]
        public String Name { get; set; }

        [Display(Name = "问题单号"), Required]
        public String Name { get; set; }

        [Display(Name = "求助人"),  Required]
        public String Applier { get; set; }

        [Display(Name = "解决人"),  Required]
        public String Solver { get; set; }

        [Display(Name = "问题标题"), MaxLength(200), Required]
        public String Question { get; set; }

        [Display(Name = "提问时间"),  Required]
        public DateTime StartDay { get; set; }

        [Display(Name = "提问内容"), Required]
        public String Content { get; set; }

    }
    /// <summary>
    /// 关键字
    /// </summary>
    public class Keyword : EntityBase
    {
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "关键字")]
        public string Keyword { get; set; }

        [Display(Name = "关键字"), Required]
        public string Keyword { get; set; }

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
    }
}
