using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Entity;
using Utl;
using System.Data;
using Model;
using System.Web.Security;
using System.Web.Profile;

namespace Sales.Controllers
{
    public class EmployeeRoleController : Controller
    {

        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult index()
        {
            //Utl.Utl.SyncUser();
            return View();

        }
        [GridAction]
        public ActionResult _SelectIndex()
        {
            return View(new GridModel(getData().OrderBy(o=>o.AccountName)));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var item = CH.DB.EmployeeRoles.Find(id);
            if (TryUpdateModel(item))
            {
                CH.DB.Entry(item).State = EntityState.Modified;
                CH.DB.SaveChanges();
            }

            return View(new GridModel(getData()));
        }
        [AcceptVerbs(HttpVerbs.Post)]

        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            var item = new EmployeeRole();

            if (TryUpdateModel(item))
            {
                CH.DB.Set<EmployeeRole>().Add(item);
                CH.DB.SaveChanges();
            }
            return View(new GridModel(getData()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            EmployeeRole record = CH.DB.Set<EmployeeRole>().Find(id);
            CH.DB.Set<EmployeeRole>().Remove(record);
            CH.DB.SaveChanges();
            return View(new GridModel(getData()));
        }

        private List<AjaxEmployee> getData()
        {

            var list = from l in CH.DB.EmployeeRoles
                       select new AjaxEmployee
                       {
                           ID = l.ID,
                           IsActivated = l.IsActivated,
                           AccountName = l.AccountName,
                           DepartmentName = (l.Department != null ? l.Department.Name : string.Empty),
                           RoleName = (l.Role != null ? l.Role.Name : string.Empty),
                           ExpLevelName = (l.ExpLevel != null ? l.ExpLevel.Name : string.Empty),
                           Email = l.Email,
                           StartDate = l.StartDate,
                           DepartmentID = l.DepartmentID,
                           ExpLevelID = l.ExpLevelID,
                           RoleID = l.RoleID,
                           AccountNameCN = l.AccountNameCN,
                           AgentNum = l.AgentNum
                       };

            var role = Employee.CurrentRole;

            if (role != null)
            {
                if (role.Level < 500)
                {
                    list = list.Where(s => s.AccountName == Employee.CurrentUserName);
                }
                else
                {
                    //list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();
                    if (role.Level == 500)
                    {
                        var mems = CH.GetAllData<Member>(m => m.Project.Manager == Employee.CurrentUserName).Select(s => s.Name).Distinct();
                        list = list.Where(f => mems.Contains(f.AccountName) || f.AccountName == Employee.CurrentUserName);
                    }
                }
            }

            return list.ToList();
        }

        public ActionResult GetAccountInfo(int? id)
        {
            return PartialView("AccountInfoWindow", getEmployeeData(id));
        }

        public ActionResult GetSetRole(int? id)
        {
            return PartialView("SetRoleWindow", getEmployeeData(id));
        }

        private AjaxEmployee getEmployeeData(int? id)
        {
            var selAccount = CH.GetDataById<EmployeeRole>(id);
            return new AjaxEmployee()
             {
                 ID = selAccount.ID,
                 AccountName = selAccount.AccountName,
                 AccountNameCN = selAccount.AccountNameCN,
                 AgentNum = selAccount.AgentNum,
                 BirthDay = selAccount.BirthDay,
                 DepartmentID = selAccount.DepartmentID,
                 DepartmentName = selAccount.Department == null ? string.Empty : selAccount.Department.Name,
                 Email = selAccount.Email,
                 ExpLevelID = selAccount.ExpLevelID,
                 ExpLevelName = selAccount.ExpLevel == null ? string.Empty : selAccount.ExpLevel.Name,
                 IsActivated = selAccount.IsActivated,
                 Gender = selAccount.Gender,
                 IsTrainee = selAccount.IsTrainee,
                 Mobile = selAccount.Mobile,
                 RoleID = selAccount.RoleID,
                 RoleName = selAccount.Role == null ? string.Empty : selAccount.Role.Name,
                 StartDate = selAccount.StartDate
             };
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return Json("");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            return Json("error");
        }

        [HttpPost]
        [ManagerRequired]
        public ActionResult AddAccount(UserInfoModel model, string AccountNameCN)
        {
            if (model.UserName.Trim().Contains(" "))
            {
                ModelState.AddModelError("UserName", "帐号中间不可以有空格.");
                return Json("帐号中间不可以有空格");
            }
            if (string.IsNullOrWhiteSpace(AccountNameCN))
            {
                ModelState.AddModelError("UserName", "中文名称不能为空.");
                return Json("中文名称不能为空");
            }

            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName.Trim(), model.Password.Trim(), model.Email.Trim(), null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    ProfileBase objProfile = ProfileBase.Create(model.UserName.Trim());

                    objProfile.SetPropertyValue("RoleLevelID", 0);
                    objProfile.SetPropertyValue("IsActivated", false);

                    var emprole = new EmployeeRole()
                    {
                        AccountName = model.UserName,
                        Email = model.Email,
                        AccountNameCN = AccountNameCN,
                        IsActivated = false
                    };
                    CH.Create<EmployeeRole>(emprole);
                    return Json("");
                }
                else
                {
                    string errorMsg = ErrorCodeToString(createStatus);
                    ModelState.AddModelError("", errorMsg);
                    return Json(errorMsg);
                }
            }
            else
            {
                return Json("验证错误");
            }
        }

        [HttpPost]
        public ActionResult AccountInfo(AjaxEmployee model)
        {
            if (model.AccountName == null)
            {
                return Json("");
            }
            if (ModelState.IsValid)
            {
                var s = CH.GetDataById<EmployeeRole>(model.ID);
                //s.AccountNameCN = model.AccountNameCN;
                s.AgentNum = model.AgentNum;
                s.Gender = model.Gender;
                s.Email = model.Email;
                s.BirthDay = model.BirthDay;
                s.DepartmentID = model.DepartmentID;
                CH.Edit<EmployeeRole>(s);
                return Json("");
            }
            else
            {
                string error = "";
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        error += state.Errors.First().ErrorMessage + ",";
                    }
                }
                return Json(error.Substring(0, error.Length - 1));
            }
        }

        [HttpPost]
        public ActionResult SetRole(AjaxEmployee model)
        {
            if (ModelState.IsValid)
            {
                var s = CH.GetDataById<EmployeeRole>(model.ID);
                s.IsActivated = model.IsActivated;
                s.RoleID = model.RoleID;
                s.ExpLevelID = model.ExpLevelID;
                s.StartDate = model.StartDate;
                s.IsTrainee = model.IsTrainee;

                //MembershipUser user = Membership.GetUser(s.AccountName);
                ProfileBase objProfile = ProfileBase.Create(s.AccountName);
                objProfile.SetPropertyValue("IsActivated", model.IsActivated);
                objProfile.Save();

                CH.Edit<EmployeeRole>(s);
                return Json("");
            }
            else
            {
                string error = "";
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        error += state.Errors.First().ErrorMessage + ",";
                    }
                }
                return Json(error.Substring(0, error.Length - 1));
            }
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
