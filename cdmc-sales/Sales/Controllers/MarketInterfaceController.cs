using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utl;
using System.IO;
using System.Text;
using Entity;

namespace Sales.Controllers
{
    [MarketInterfaceRequired( AccessType=AccessType.Equal)]
    public class MarketInterfaceController : Controller
    {
      
        public ViewResult MarketIndex()
        {
            return View(CH.GetAllData<Lead>());
        }


 
        public ActionResult EmailExportCsv(int page, string orderBy, string filter)
        {
            var leads = CH.GetAllData<Lead>();
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            foreach (var lead in leads)
            {
                writer.Write(lead.EMail);
                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "Emails.csv");
        }

    }
}
