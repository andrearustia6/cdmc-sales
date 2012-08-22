using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Entity;
using System.Web.Profile;
using Utl;

namespace MvcGlobalAuthorize.Controllers
{
    public class AccountController : Controller
    {


        //
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
                        return RedirectToAction("Index", "Home");
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

        //
        [ManagerRequired]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
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
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
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
                um.Contact = objProfile.GetPropertyValue("Contact") as string;
                um.Mobile = objProfile.GetPropertyValue("Mobile") as string;
                um.Gender = objProfile.GetPropertyValue("Gender") as string;
                um.DisplayName = objProfile.GetPropertyValue("DisplayName") as string;
                int roleid;
                data = objProfile.GetPropertyValue("RoleLevelID");
                Int32.TryParse(data.ToString(), out roleid);
                um.RoleLevelID = roleid;
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
                    objProfile.Save();
                    return RedirectToAction("Index");
                }

            }

            return View(model);
        }


        public ActionResult SetRoleLevel(string name)
        {
            var um = new UserInfoModel();

            ProfileBase objProfile = ProfileBase.Create(name);
            object data = null;
            int roleid;
            data = objProfile.GetPropertyValue("RoleLevelID");
            Int32.TryParse(data.ToString(), out roleid);
            um.RoleLevelID = roleid;
            um.UserName = name;
            return View(um);
        }

        [HttpPost]
        public ActionResult SetRoleLevel(UserInfoModel model)
        {
            if (Employee.IsEqualToCurrentUserName(model.UserName) || model.RoleLevelID>0)
            {
                ModelState.Remove("Password");
                ModelState.Remove("UserName");
                if (ModelState.IsValid)
                {
                    MembershipUser user = Membership.GetUser(model.UserName);

                    user.Email = model.Email;
                    Membership.UpdateUser(user);

                    ProfileBase objProfile = ProfileBase.Create(model.UserName);
                    objProfile.SetPropertyValue("RoleLevelID", model.RoleLevelID);
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
            if (role.Level < 1000 && role.Level>=500)
                list = list.FindAll(i => Employee.GetRoleLevel(i.UserName)<=500);
            else if (role.Level < 500)
                list = list.FindAll(i => Employee.IsEqualToCurrentUserName(i.UserName));
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
