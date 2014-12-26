using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MyR.Filters;
using MyR.Models;
using MyE;
using System.IO;

namespace MyR.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #region Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserEmail, model.Password, new { Question = model.Question, Answer = model.Answer, CreateDate = DateTime.Now });
                    //WebSecurity.CreateUserAndAccount(model.UserEmail, model.Password);
                    WebSecurity.Login(model.UserEmail, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        #region Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            
            if (ModelState.IsValid && WebSecurity.Login(model.UserEmail, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserEmail = result.UserName, ExternalLoginData = loginData });
            }
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (AccountContext db = new AccountContext())
                {
                    RUser user = db.RUser.FirstOrDefault(u => u.RUserEmail.ToLower() == model.UserEmail.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.RUser.Add(new RUser { RUserEmail = model.UserEmail });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserEmail);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return RedirectToAction("Index", "Home");
        }

        #region Recover Password
        [AllowAnonymous]
        public ActionResult PasswordRecovery()
        {
            PasswordRecoveryModel model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecovery(PasswordRecoveryModel model)
        {
            
            if (!ModelState.IsValid)
                return View(model);

            RUser user = RUser.GetUserByEmail(model.UserEmail);
            //user == null: user doesn't exist
            //user has no question: go to your provider to recover your password
            if (user == null)
                ModelState.AddModelError("NoUser", "This user email doesn't exist in the Review.");
            else if (user != null && !OAuthWebSecurity.HasLocalAccount(user.RUserId))
                ModelState.AddModelError("FromProvider", "This user email is not registered under Review. Please recover your password from the site where you registered.");
            else if (OAuthWebSecurity.HasLocalAccount(user.RUserId) && string.IsNullOrEmpty(model.Answer))
                ModelState.AddModelError("NoAnswer", "Please provide an answer.");
            else if (user != null && string.IsNullOrEmpty(model.Question))
            {
                model.Question = user.Question;
                model.ButtonText = "Submit";
            }
            else
            {
                if (user.RUserEmail == model.UserEmail && user.Question == model.Question && user.Answer == model.Answer)
                {
                    string pwResetToken = WebSecurity.GeneratePasswordResetToken(user.RUserEmail); //token expiry default is 24 hrs.
                    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                    string changePasswordLink = baseUrl + "Account/PasswordRecovery2/?Token=" + pwResetToken;
                    MyEmail.ChangePasswordEmail(user.RUserEmail, changePasswordLink, Constants.SystemConstants.PasswordResetTokenExpiryDuration, string.Empty); //TODO: add first, middle, last name into RUser
                    
                    model.NoticeMessage = "A new password has been sent to your email. You may change it to your password when you log in.";
                }
                else
                    ModelState.AddModelError("WrongAnswer", "Wrong Answer");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult PasswordRecovery2(string Token)
        {
            RUser user = RUser.GetUserByChangePasswordToken(Token);
            ViewBag.TokenUser = user;
            if (user == null)
            {
                ModelState.AddModelError("InvalidToken", "This link may not be valid or expired. Please try to recover your password again.");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecovery2(string Token, PasswordRecovery2Model model)
        {
            RUser user = RUser.GetUserByChangePasswordToken(Token);
            if (user != null)
            {
                WebSecurity.ResetPassword(Token, model.Password);
                return RedirectToAction("PasswordRecoverySuccess");
            }
            //something is wrong if code reaches here:
            ModelState.AddModelError("NoUser", "Unknown error. Please request another recover password.");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult PasswordRecoverySuccess()
        {
            return View();
        }
        #endregion

        #region Profile Management
        public ActionResult Manage()
        {
            ManageModel manageModel = new ManageModel(User.Identity.Name);
            if (OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name)))
                ViewBag.ChangePasswordText = "Change password";
            else
                ViewBag.ChangePasswordText = "Add a local password";
            return View(manageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageModel model, HttpPostedFileBase profile_file)
        {
            RUser rUser = RUser.GetUserByEmail(model.RUser.RUserEmail);
            if (ModelState.IsValid && rUser != null)
            {
                string filePath = string.Empty;
                if (profile_file != null)
                {
                    string ext = Path.GetExtension(profile_file.FileName);
                    string virtualPath = "/SiteFiles/" + rUser.RUserId + "/";
                    string physicalPath = Server.MapPath(virtualPath);
                    if (!Directory.Exists(physicalPath))
                        Directory.CreateDirectory(physicalPath);
                    profile_file.SaveAs(physicalPath + "profilePic" + ext);
                    model.RUser.PhotoPath = virtualPath + "profilePic" + ext;
                }
                bool saved = RUser.SaveUser(rUser, model.RUser.Question, model.RUser.Answer, model.RUser.FirstName,
                    model.RUser.MiddleName, model.RUser.LastName, model.RUser.Gender, model.RUser.Phone, model.RUser.PhotoPath);

                if (saved)
                {
                    ViewBag.MessageCss = "notice_message";
                    ViewBag.Message = "Saved Successfully!";
                }
                else
                {
                    ViewBag.MessageCss = "error_message";
                    ViewBag.Message = "Saved Failed. Please try again.";
                }
                ViewBag.ChangePasswordText = "Change password";
            }
            return View(model);
        }
        #endregion

        #region Change Password
        
        public ActionResult ChangePassword()
        {
            int userId = WebSecurity.GetUserId(User.Identity.Name);
            bool hasLocalPassword = OAuthWebSecurity.HasLocalAccount(userId);
            SetChangePasswordViewBags(hasLocalPassword, "message-success");
            LocalPasswordModel model = new LocalPasswordModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            int userId = WebSecurity.GetUserId(User.Identity.Name);
            bool hasLocalPassword = OAuthWebSecurity.HasLocalAccount(userId);
            if (!ModelState.IsValid)
            {
                SetChangePasswordViewBags(hasLocalPassword, "message-success");
                return View(model);
            }

            bool canChange = model.NewPassword == model.ConfirmPassword;
            if (!canChange)
            {
                SetChangePasswordViewBags(hasLocalPassword, "error_message", "Confirm password mismatch.");
                LocalPasswordModel newModel = new LocalPasswordModel();
                return View(newModel);
            }

            MembershipUser member = Membership.GetUser(User.Identity.Name);
            if (hasLocalPassword)
            {
                bool changed = member.ChangePassword(model.OldPassword, model.NewPassword);
                if (!changed)
                {
                    SetChangePasswordViewBags(hasLocalPassword, "error_message", "Incorrect current password.");
                    LocalPasswordModel newModel = new LocalPasswordModel();
                    return View(newModel);
                }
            }   
            else
            { 
                //create a local account
                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                WebSecurity.Login(User.Identity.Name, model.NewPassword);
            }

            SetChangePasswordViewBags(hasLocalPassword, "message-success", "Password changed successfully");
            return View(model);
        }

        private void SetChangePasswordViewBags(bool hasLocalPassword, string messageCss, string statusMessage = null)
        {
            ViewBag.HasLocalPassword = hasLocalPassword;
            ViewBag.ReturnUrl = "Manage";
            ViewBag.MessageCss = messageCss;
            if (statusMessage != null)
                ViewBag.StatusMessage = "Password changed successfully";
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        #region Helpers
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        #endregion
    }
}
