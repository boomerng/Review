using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace MyR.Models
{
    public partial class RReview
    {
        internal static RReview GetReviewById(int reviewId)
        {
            using (MRContext db = new MRContext())
            {
                return db.RReview.Where(x => x.RReviewId == reviewId).SingleOrDefault();
            }
        }

        internal static RReview GetReviewById(int reviewId, MRContext db)
        {
            return db.RReview.Where(x => x.RReviewId == reviewId).SingleOrDefault();
        }

        internal static List<RReview> GetReviewsByUserId(int userId, int skip, int take,
            string orderby = Constants.DBFields.fLastModifiedDate,
            bool isAscending = false)
        {
            using (MRContext db = new MRContext())
            {
                var result = db.RReview.Where(x => x.CreateUserId == userId);
                if (orderby == Constants.DBFields.fLastModifiedDate)
                {
                    if (isAscending)
                        result = result.OrderBy(x => x.LastModifiedDate);
                    else
                        result = result.OrderByDescending(x => x.LastModifiedDate);
                }
                else if (orderby == Constants.RReview.fCategoryId)
                {
                    if (isAscending)
                        result = result.OrderBy(x => x.RCategoryId);
                    else
                        result = result.OrderByDescending(x => x.RCategoryId);
                }
                else
                {
                    if (isAscending)
                        result = result.OrderBy(x => x.RReviewId);
                    else
                        result = result.OrderByDescending(x => x.RReviewId);
                }
                if (take < 0) //take all
                    result = result.Skip(skip);
                else
                    result = result.Skip(skip).Take(take);

                return result.ToList();
            }
        }

        internal static RReview SaveReview(int reviewId, int categoryId, string itemName,
            string brandName, float rating, string title, string content, int userId, string newCategoryName)
        {
            using (MRContext db = new MRContext())
            {
                RReview toBeSaved = new RReview();
                EntityState theState = EntityState.Added;
                if (reviewId != 0)
                {
                    toBeSaved = GetReviewById(reviewId);
                    theState = EntityState.Modified;
                }

                if (!string.IsNullOrEmpty(newCategoryName))
                {
                    RCategory category = RCategory.InsertCategory(newCategoryName, userId, db);
                    toBeSaved.Category = category;
                }
                else
                    toBeSaved.RCategoryId = categoryId;
                
                toBeSaved.ItemName = itemName;
                toBeSaved.BrandName = brandName;
                toBeSaved.Rating = rating;
                toBeSaved.Title = title;
                toBeSaved.BodyContent = content;

                if (theState == EntityState.Added)
                {
                    toBeSaved.CreateDate = DateTime.Now;
                    toBeSaved.CreateUserId = userId;
                }
                toBeSaved.LastModifiedDate = DateTime.Now;
                toBeSaved.LastModifiedUserId = userId;
                db.Entry(toBeSaved).State = theState;
                db.SaveChanges();
                
                return toBeSaved;
            }
        }
    }
}