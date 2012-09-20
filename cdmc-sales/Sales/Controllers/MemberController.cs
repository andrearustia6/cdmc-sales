using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Sales;
using Utl;
using BLL;

namespace Sales.Controllers
{
    public class MemberController : Controller
    {

        public ViewResult Index()
        {
            return View(CH.GetAllData<Member>());
        }

        public ViewResult Details(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }

        public ActionResult Distribution(int? projectid)
        {
            var member = CH.GetAllData<Member>(i => i.ProjectID == projectid);
            var dc= CRM_Logical.GetDefaultCharatracter();
            member.ForEach(m => { 
                dc.RemoveAll(i => m.CharactersSet.Contains(i.ToUpper()));
            });
            ViewBag.ProjectID = projectid;
            ViewBag.DC = dc;

            return View(member);
        }

        [HttpPost]
        public ActionResult Distribution(List<string> mc,int? projectid)
        {
            //清空原有的字头分配
            List<Member> temp = CH.GetAllData<Member>(m => m.ProjectID == projectid);
            temp.ForEach(m =>
            {
                m.Characters = string.Empty;
            });

            if (mc != null)
            {
                mc.ForEach(child =>
                {
                    //解析提交的数据
                    var data = child.Split('|');
                    var id = Int32.Parse(data[0]);
                    var value = data[1];

                    
                    Member member = temp.FirstOrDefault(m => m.ID == id);

                    if (string.IsNullOrEmpty(member.Characters))
                        member.Characters = value;
                    else
                        member.Characters += "|" + value;
                });
                
            }

            //把更新的charatacter写到数据库
            temp.ForEach(tm =>
            {
                CH.Edit<Member>(tm);
            });

            return RedirectToAction("Management", "Project", new { id = projectid });
          
        }

        public ActionResult Create(int? projectid)
        {
            ViewBag.ProjectID = projectid;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Member item)
        {
            if (!CRM_Logical.IsMemberExist(item.Name))
            {
                ModelState.AddModelError("", "该员工不存在");
            }
            if (CRM_Logical.IsSameMemberExistInProject(item.Name, item.ProjectID))
            {
                ModelState.AddModelError("", "该员工以加入此项目");
            }

            if (ModelState.IsValid)
            {
                CH.Create<Member>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
                //return View(@"~\views\Project\Management.cshtml", CH.GetDataById<Project>(item.ProjectID));
            }
            ViewBag.ProjectID = item.ProjectID;
            return View(item);
        }
        public ActionResult Edit(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }

        [HttpPost]
        public ActionResult Edit(Member item)
        {
            if (!CRM_Logical.IsMemberExist(item.Name))
            {
                ModelState.AddModelError("", "该员工不存在");
            }
            //var memberoldname = CH.DB.Members.AsNoTracking().FirstOrDefault(i => i.ID == item.ID);
            //if (CRM_Logical.IsSameMemberExistInProject(item.Name, item.ProjectID) && memberoldname.Name != item.Name)
            //{
            //    ModelState.AddModelError("", "该员工以加入此项目");
            //}
            //else
            //{
            //    CH.DB.Entry(memberoldname).State = EntityState.Detached; 
            //    CH.DB.Members.Attach(item);
            //}
            if (ModelState.IsValid)
            {
                CH.Edit<Member>(item);
                return RedirectToAction("Management", "Project", new { id = item.ProjectID });
            }
            return View(item);
        }

        public ActionResult Delete(int id)
        {
            return View(CH.GetDataById<Member>(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var item = CH.GetDataById<Member>(id);
            var pid = item.ProjectID;
            CH.Delete<Member>(id);
            return RedirectToAction("Management", "Project", new { id = pid });
        }
    }
}