﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Entity;
using System.Web.Profile;
using Utl;
using BLL;

namespace MvcGlobalAuthorize.Controllers
{
    public class AccountController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            CH.DB.Dispose();
            base.Dispose(disposing);
        }

        // GET: /Account/LogOn
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            var activated = (bool)Employee.GetProfile("IsActivated", model.UserName);
            if (!activated && model.UserName != "karen")
            {
                ModelState.AddModelError("","该账号为非激活状态,请联系管理员");
            }
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                  
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        //if (Employee.EqualToLeader() || Employee.EqualToSales())
                        //    return RedirectToAction("mypage", "sales");
                        //else
                            return RedirectToAction("Index", "account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户名或者密码不正确.");
                }
            }

            // If we got this far, something failed, redisplay form 103691：86500
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOn", "account");
        }

        [AdministratorRequired]
        public ActionResult ResetPassword(string username)
        {
            if (username == null) return View();
            var user = Membership.GetUser(username);
            if (user != null)
            {
                return View("ResetPassword", "", user.ResetPassword());
            }
            else
                return View("ResetPassword","" ,"找不到对应的用户");
        }

        [AllowAnonymous]
        public ActionResult Start()
        {
            MembershipCreateStatus createStatus;

            ProfileBase objProfile = ProfileBase.Create("karen");
            Membership.CreateUser("karen", "111111", "karen@cdmc.org.cn", null, null, true, null, out createStatus);
            objProfile.SetPropertyValue("RoleLevelID", 1);
            objProfile.SetPropertyValue("IsActivated", true);
            objProfile.Save();

            Membership.CreateUser("flora", "111111", "flora@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase flora = ProfileBase.Create("flora");
            flora.SetPropertyValue("RoleLevelID", 5);
            flora.SetPropertyValue("IsActivated", true);
            flora.Save();

            Membership.CreateUser("mike", "111111", "mike@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase mike = ProfileBase.Create("mike");
            mike.SetPropertyValue("RoleLevelID", 2);
            mike.SetPropertyValue("IsActivated", true);
            mike.Save();

            Membership.CreateUser("sean", "111111", "sean@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase sean = ProfileBase.Create("sean");
            sean.SetPropertyValue("RoleLevelID", 3);
            sean.SetPropertyValue("IsActivated", true);
            sean.Save();

            Membership.CreateUser("stone", "111111", "stone@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase stone = ProfileBase.Create("stone");
            stone.SetPropertyValue("RoleLevelID", 3);
            stone.SetPropertyValue("IsActivated", true);
            stone.Save();

            Membership.CreateUser("susie", "111111", "susie@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase susie = ProfileBase.Create("susie");
            susie.SetPropertyValue("RoleLevelID", 4);
            susie.SetPropertyValue("IsActivated", true);
            susie.Save();

            Membership.CreateUser("john", "111111", "john@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase john = ProfileBase.Create("john");
            john.SetPropertyValue("RoleLevelID", 4);
            john.SetPropertyValue("IsActivated", true);
            john.Save();

            Membership.CreateUser("tina", "111111", "tina@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase tina = ProfileBase.Create("tina");
            tina.SetPropertyValue("RoleLevelID", 4);
            tina.SetPropertyValue("IsActivated", true);
            tina.Save();

            Membership.CreateUser("lucas", "111111", "lucas@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase lucas = ProfileBase.Create("lucas");
            lucas.SetPropertyValue("RoleLevelID", 4);
            lucas.SetPropertyValue("IsActivated", true);
            lucas.Save();

            Membership.CreateUser("amy", "111111", "amy@cdmc.org.cn", null, null, true, null, out createStatus);
            ProfileBase amy = ProfileBase.Create("amy");
            amy.SetPropertyValue("RoleLevelID", 7);
            amy.SetPropertyValue("IsActivated", true);
            amy.Save();

            return RedirectToAction("Index", "account");
        }
        //
        [ManagerRequired]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Details(string name)
        {
            if (Employee.IsEqualToCurrentUserName(name))
                return RedirectToAction("UpdateProfile", "account", new { name = name });

            var membershipuser = Membership.GetUser(name);

            var um = new UserInfoModel();
            um.UserName = membershipuser.UserName;
            um.Email = membershipuser.Email;

            ProfileBase objProfile = ProfileBase.Create(membershipuser.UserName);
            DateTime b;
            object data = null;
            data = objProfile.GetPropertyValue("BirthDay");
            DateTime.TryParse(data.ToString(), out b);
            um.BirthDay = b;
            int contact = 0;
            Int32.TryParse(objProfile.GetPropertyValue("Contact").ToString(),out contact);
            um.Contact = contact;
            um.Mobile = objProfile.GetPropertyValue("Mobile") as string;
            um.Gender = objProfile.GetPropertyValue("Gender") as string;
            um.DisplayName = objProfile.GetPropertyValue("DisplayName") as string;
            int roleid;
            data = objProfile.GetPropertyValue("RoleLevelID");
            Int32.TryParse(data.ToString(), out roleid);
            um.RoleID = roleid;
            return View(um);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ManagerRequired]
        public ActionResult Register(UserInfoModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    ProfileBase objProfile = ProfileBase.Create(model.UserName);

                    objProfile.SetPropertyValue("RoleLevelID", 0);
                    objProfile.SetPropertyValue("IsActivated", false);
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult UpdateProfile(string name)
        {
            if (Employee.IsEqualToCurrentUserName(name))
            {
                var membershipuser = Membership.GetUser(name);

                var um = new UserInfoModel();
                um.UserName = membershipuser.UserName;
                um.Email = membershipuser.Email;

                ProfileBase objProfile = ProfileBase.Create(membershipuser.UserName);
                DateTime b;
                object data = null;
                data = objProfile.GetPropertyValue("BirthDay");
                DateTime.TryParse(data.ToString(), out b);
                um.BirthDay = b;
                int con = 0;
                Int32.TryParse(objProfile.GetPropertyValue("Contact").ToString(),out con);
                int expid = 0;
                Int32.TryParse(objProfile.GetPropertyValue("ExpLevelID").ToString(), out expid);
                um.ExpLevelID = expid;
                um.Contact = con;
                um.Mobile = objProfile.GetPropertyValue("Mobile") as string;
                um.Gender = objProfile.GetPropertyValue("Gender") as string;
                um.DisplayName = objProfile.GetPropertyValue("DisplayName") as string;
                um.Department = objProfile.GetPropertyValue("Department") as string;
                int roleid;
                data = objProfile.GetPropertyValue("RoleLevelID");
                Int32.TryParse(data.ToString(), out roleid);
                um.RoleID = roleid;
                return View(um);
            }
            else
                return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UpdateProfile(UserInfoModel model)
        {
            if (Employee.IsEqualToCurrentUserName(model.UserName))
            {
                ModelState.Remove("Password");
                ModelState.Remove("UserName");
                if (ModelState.IsValid)
                {
                    MembershipUser user = Membership.GetUser(model.UserName);

                    user.Email = model.Email;
                    Membership.UpdateUser(user);

                    ProfileBase objProfile = ProfileBase.Create(model.UserName);

                    objProfile.SetPropertyValue("Contact", model.Contact);
                    objProfile.SetPropertyValue("Mobile", model.Mobile);
                    objProfile.SetPropertyValue("Gender", model.Gender);
                    objProfile.SetPropertyValue("BirthDay", model.BirthDay);
                    objProfile.SetPropertyValue("DisplayName", model.DisplayName);
                    objProfile.SetPropertyValue("Department", model.Department);
                    objProfile.Save();
                    return RedirectToAction("Index");
                }

            }

            return View(model);
        }

        [DirectorRequired]
        public ActionResult SetRoleLevel(string name)
        {
            var um = new UserInfoModel();
            um.UserName = name;

            ProfileBase objProfile = ProfileBase.Create(name);
            object data = null;
            int roleid;
            data = objProfile.GetPropertyValue("RoleLevelID");
            Int32.TryParse(data.ToString(), out roleid);
            um.RoleID = roleid;
            
            object StartDate;
            DateTime startdate;
            StartDate = objProfile.GetPropertyValue("StartDate");
            DateTime.TryParse(StartDate.ToString(), out startdate);
            um.StartDate = startdate;
       

            object activate = null;
            bool isactivated;
            activate = objProfile.GetPropertyValue("IsActivated");
            Boolean.TryParse(activate.ToString(), out isactivated);
            um.IsActivated = isactivated;

            object explevel = null;
            int explevelid;
            explevel = objProfile.GetPropertyValue("ExpLevelID");
            Int32.TryParse(explevel.ToString(), out explevelid);
            um.ExpLevelID = explevelid;

            return View(um);
        }

        [HttpPost]
        [DirectorRequired]
        public ActionResult SetRoleLevel(UserInfoModel model)
        {
            if (Employee.IsEqualToCurrentUserName(model.UserName) || model.RoleID>0)
            {
                ModelState.Remove("Password");
                ModelState.Remove("UserName");
                if ((model.RoleID == 0 || model.StartDate==null) && model.IsActivated == true)
                    ModelState.AddModelError("","激活的账号必须设置职级和入职日期才能生效");
                if (ModelState.IsValid)
                {
                    MembershipUser user = Membership.GetUser(model.UserName);

                    user.Email = model.Email;
                    Membership.UpdateUser(user);

                    ProfileBase objProfile = ProfileBase.Create(model.UserName);
                    objProfile.SetPropertyValue("RoleLevelID", model.RoleID);
                    objProfile.SetPropertyValue("ExpLevelID", model.ExpLevelID);
                    objProfile.SetPropertyValue("IsActivated", model.IsActivated);
                    objProfile.SetPropertyValue("StartDate", model.StartDate.Value.ToShortDateString());
                    objProfile.Save();
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        public string TopSecret()
        {
            return "Red, Blue, Green";
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
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
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        public ActionResult Index()
        {
            var level = (int)Employee.GetCurrentProfile("RoleLevelID");
            var role = CH.GetDataById<Role>(level);
            var list = Membership.GetAllUsers().Cast<MembershipUser>().ToList<MembershipUser>();
            if (role.Level >= 0 && role.Level < 500)
                list = list.FindAll(i => Employee.IsEqualToCurrentUserName(i.UserName));
            else if (role.Level>=500)
                list = list.FindAll(i => Employee.GetRoleLevel(i.UserName) <= role.Level);
          
            return View(list);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
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
        #endregion
    }
}
