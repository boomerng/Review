using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyR.Constants
{
    public class SystemConstants
    {
        public const int PasswordLength = 8;
        public const int PasswordResetTokenExpiryDuration = 1;
        public const string DefaultProfilePhotoPath = "/Images/Shared/default_profile.jpg";

        public const int SystemUserId = 0;

        public const string CancelButton = "Cancel";
    }

    public class MyProfile
    {
        public const string Male = "male";
        public const string Female = "female";
    }
    /// <summary>
    /// Commonly used fields
    /// </summary>
    public class DBFields
    {
        public const string fCreateDate = "CreateDate";
        public const string fLastModifiedDate = "LastModifiedDate";
        public const string fCreateUserId = "CreateUserId";
        public const string fLastModifiedUserId = "LastModifiedUserId";
    }

    public class RReview
    {
        public const string fCategoryId = "RCategoryId";
        public const string fCategoryName = "CategoryName";
    }
}