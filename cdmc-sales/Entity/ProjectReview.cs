using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Entity
{
    public class ProjectReview:EntityBase
    {
        public string Summary { get; set; }

        public int? ProjectID { get; set; }
        public virtual Project Project { get; set; }

    }
}
