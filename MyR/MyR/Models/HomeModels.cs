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

namespace MyR.Models
{
    public class HomeModel
    {
        public RUser LoginUser {get;set;}
        public ProfilePhotoModel ProfilePhoto { get; set; }
        public ProfileCategoriesModel ProfileCategories { get; set; }
        public List<RReview> ProfileReviewList { get; set; }

        public HomeModel()
        {
            LoginUser = new RUser();
            ProfilePhoto = new ProfilePhotoModel();
            ProfileCategories = new ProfileCategoriesModel();
        }

        public HomeModel(string email)
           : this()
        {
            LoginUser = RUser.GetUserByEmail(email);
            if (LoginUser != null)
            {
                ProfilePhoto = new ProfilePhotoModel(LoginUser.PhotoPath);
                ProfileCategories = new ProfileCategoriesModel(LoginUser.RUserId);
                ProfileReviewList = RReview.GetReviewsByUserId(LoginUser.RUserId, 0, 20);
            }
        }
        
    }
    public class ProfilePhotoModel
    {
        public string ProfilePhotoPath { get; set; }

        public ProfilePhotoModel()
        { }

        public ProfilePhotoModel(string virtualPath)
        {
            string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);
            if (string.IsNullOrEmpty(virtualPath) || !File.Exists(physicalPath))
                ProfilePhotoPath = Constants.SystemConstants.DefaultProfilePhotoPath;
            else
                ProfilePhotoPath = virtualPath;
                
        }
    }

    public class ProfileCategoriesModel
    {
        public List<RCategory> Categories { get; set; }

        public ProfileCategoriesModel() 
        {
            Categories = new List<RCategory>();
        }

        public ProfileCategoriesModel(int userId)
        {
            Categories = RCategory.GetCategoriesByUserReview(userId);
        }
    }
}