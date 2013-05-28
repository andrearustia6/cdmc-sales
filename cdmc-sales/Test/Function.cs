using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;

namespace Test
{
    [TestClass]
    public class Function
    {
        [TestMethod]
        public void TestCompanyImport()
        {
            var source =new int[]{34};
            CRM_Logical.CRM.ImportCompany(source,72,"Alextre");
        }
    }
}
