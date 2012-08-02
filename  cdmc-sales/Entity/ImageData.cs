using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class ClientImage:EntityBase
    {
        [Column(TypeName = "image")]
        public Byte[] ImageData { get; set; }

        [Display(Name = "文件类型")]
        public string ContentType { get; set; }

        [Display(Name = "图片名称")]
        public string Name { get; set; }

       
    }
}
