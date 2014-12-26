using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.ComponentModel;
using MyR.Filters;

namespace MyR.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "User email")]
        [Email(ErrorMessage = "Invalid Email")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string UserEmail { get; set; }

        [Required]
        [Password(ErrorMessage = "Password has to be an alpha-numeric string with minimum length of 8.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Security Question")]
        [StringLength(128, ErrorMessage = "Question cannot be longer than 128 characters.")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "Security Answer")]
        [StringLength(128, ErrorMessage = "Answer cannot be longer than 128 characters.")]
        public string Answer { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User email")]
        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User email")]
        public string UserEmail { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordRecoveryModel
    {
        [Required(ErrorMessage="Please provider your email")]
        [Display(Name = "Your User Email")]
        [Email(ErrorMessage = "Invalid Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Security Question")]
        public string Question { get; set; }

        [Display(Name = "Security Answer")]
        public string Answer { get; set; }

        public string ButtonText { get; set; }

        public string NoticeMessage { get; set; }
        public PasswordRecoveryModel()
        {
            ButtonText = "Continue";
            NoticeMessage = string.Empty;
        }
    }

    public class PasswordRecovery2Model
    {
        [Required]
        [Password(ErrorMessage = "Password has to be an alpha-numeric string with minimum length of 8.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ManageModel
    {
        public RUser RUser { get; set; }
        public SelectList GenerList { get; set; }
        
        public ManageModel() 
        {
            GenerList = new SelectList(new[] 
                { new SelectListItem { Text=MyR.Constants.MyProfile.Male, Value=MyR.Constants.MyProfile.Male },
                  new SelectListItem { Text=MyR.Constants.MyProfile.Female, Value=MyR.Constants.MyProfile.Female }
                });
        }

        public ManageModel(string userEmail) : this()
        {
            RUser = RUser.GetUserByEmail(userEmail);
        }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
