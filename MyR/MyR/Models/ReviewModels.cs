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
using System.Linq;

namespace MyR.Models
{
    public class ReviewModel
    {
        public RReview Review { get; set; }
        public SelectList Categories { get; set; }
        public string NewCategoryName { get; set; }

        public ReviewModel() 
        { 
        }

        public ReviewModel(string userEmail)
        {
            AccountContext accountContext = new AccountContext();
            MRContext mrContext = new MRContext();

            RUser user = RUser.GetUserByEmail(userEmail, accountContext);
            Review = new RReview();
            List<RCategory> tempCates = RCategory.GetSystemAndUserCategories(user.RUserId, mrContext);
            Categories = new SelectList(tempCates, Constants.RReview.fCategoryId, Constants.RReview.fCategoryName);
        }

        public ReviewModel(int reviewId, string userEmail)
            :this(userEmail)
        {
            Review = RReview.GetReviewById(reviewId);
        }
    }
}