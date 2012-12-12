using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Entity;
using Utl;
using System.Web.Profile;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

namespace Sales.Controllers
{
    [LeaderRequired]
    public class ReportController : Controller
    {
        public ActionResult MemberProgress(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var projects = this.GetProjectByAccount();
           
            return View(projects);
        }
        //
        // GET: /Report/

        public void GetDuration(DateTime startdate, DateTime enddate, List<Project> ps)
        {
            if (ps.Count > 0)
            {
                string contacts = "where convert(varchar(10) ,cast(startdate as datetime),108) between '"+ startdate.ToString()+"' and '"+enddate.ToString()+"' ";

                ps.ForEach(p =>
                {
                    p.Members.ForEach(item =>
                    {
                        ProfileBase userp = ProfileBase.Create(item.Name);
                        if (userp != null)
                        {
                            var uc = userp.GetPropertyValue("Contact");
                            if (uc != null && !string.IsNullOrEmpty(uc.ToString()))
                            {
                                int contect = 0;
                                Int32.TryParse(uc.ToString(), out contect);
                                if (contect > 0)
                                {
                                    if (string.IsNullOrEmpty(contacts))
                                        contacts = "where phone like '" + contect.ToString() + "'";
                                    else
                                        contacts += " or phone like '" + contect.ToString() + "'";

                                }
                            }
                        }
                    });

                });
                string cstr = ConfigurationManager.ConnectionStrings["BillDB"].ToString();
                using (var con = new OleDbConnection(cstr))
                {

                    string sql = "SELECT * FROM bill " + contacts;
                    OleDbDataAdapter da = new OleDbDataAdapter(sql, con);
                    OleDbCommand c = new OleDbCommand(sql, con);
                    da.SelectCommand = c;
                    DataSet ds = new DataSet();//创建数据集
                    da.Fill(ds, "bill");//填充数据集
                    DataTable tb = ds.Tables["bill"];//创建表
                    var rows = ds.Tables["bill"].Select();
                    var rs = rows.ToList();
                }
            }
        }

        public ActionResult LeadCalls(DateTime? startdate, DateTime? enddate)
        {
            if (startdate == null)
            {
                startdate = new DateTime(1, 1, 1);
            }
            if (enddate == null)
            {
                enddate = new DateTime(9999,1,1);
            }
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;


            var result = new TotalLeadCallAmount();
            var ps = this.GetProjectByAccount();
            var vl = new List<ViewLeadCallAmountInProject>();

            ps.ForEach(p =>
            {
                vl.Add(new ViewLeadCallAmountInProject() { LeadCallAmounts = p.GetProjectMemberLeadCalls(startdate, enddate), project = p });

            });
          
            result.ViewLeadCallAmountInProjects = vl;

            //ProfileBase userp = ProfileBase.Create(item.Name);
            //if (userp != null)
            //{
            //    var uc = userp.GetPropertyValue("Contact");
            //    if (uc != null && !string.IsNullOrEmpty(uc.ToString()))
            //    {
                  
            //    }
            //}
            return View(result);
        }

        public ActionResult MemberLeadCalls(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var ps = this.GetProjectByAccount();
            return View(ps);
        }

        public ActionResult Progress(DateTime? startdate, DateTime? enddate)
        {
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            var ps = this.GetProjectByAccount();
            var data = new List<ViewProjectProgressAmount>();
            ps.ForEach(p =>
            {
                var d = p.GetProjectProgress(startdate,enddate);
                data.Add(d);
            });
            return View(data);
        }

    }
}
