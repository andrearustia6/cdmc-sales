using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class UserFavorsCrmGroup : EntityBase
    {
        [Required]
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public virtual List<UserFavorsCRM> UserFavorsCRMs {get;set;}
    }

    public class UserFavorsCRM : EntityBase
    {
        public virtual CompanyRelationship CompanyRelationship { get; set; }
        [Required]
        public int? CompanyRelationshipID { get; set; }

        public virtual UserFavorsCrmGroup UserFavorsCrmGroup { get; set; }
        public int? UserFavorsCrmGroupID { get; set; }
    }
}
