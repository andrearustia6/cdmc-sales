using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entity;
using Utl;

namespace Utl
{
    public class Query
    {
        public static IQueryable<Project> AliveProjects()
        {
            return CH.DB.Projects.Where(w => w.IsActived == true);
        }

        public static IQueryable<Member> AliveMembers()
        {
            return CH.DB.Members.Where(w => w.IsActivated == true);
        }
    }
}