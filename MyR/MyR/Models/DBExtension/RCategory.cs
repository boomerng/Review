using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyR.Models
{
    public partial class RCategory
    {
        internal static RCategory GetCategoryById(int categoryId)
        {
            return GetCategoryById(categoryId, new MRContext());
        }

        internal static RCategory GetCategoryById(int categoryId, MRContext db)
        {
            return db.RCategory.Where(x => x.RCategoryId == categoryId).SingleOrDefault();
        }
        /// <summary>
        /// Get Categories created by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static List<RCategory> GetCategoriesByUserId(int userId)
        {
            return GetCategoriesByUserId(userId, new MRContext());
        }
        /// <summary>
        /// Get Categories created by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static List<RCategory> GetCategoriesByUserId(int userId, MRContext db)
        {
            return db.RCategory.Where(x => x.CreateUserId == userId).OrderBy(x=>x.CategoryName).ToList();
        }
        /// <summary>
        /// Get Categories created by system
        /// </summary>
        /// <returns></returns>
        internal static List<RCategory> GetSystemCategories()
        {
            return GetSystemCategories(new MRContext());
        }
        /// <summary>
        /// Get Categories created by system
        /// </summary>
        /// <returns></returns>
        internal static List<RCategory> GetSystemCategories(MRContext db)
        {
            return db.RCategory.Where(x => x.CreateUserId == Constants.SystemConstants.SystemUserId).OrderBy(x => x.CategoryName).ToList();
        }

        internal static List<RCategory> GetSystemAndUserCategories(int userId)
        {
            return GetSystemAndUserCategories(userId, new MRContext());
        }

        internal static List<RCategory> GetSystemAndUserCategories(int userId, MRContext db)
        {
            List<RCategory> categories = RCategory.GetCategoriesByUserId(userId, db);
            categories.AddRange(RCategory.GetSystemCategories());
            return categories.AsQueryable().OrderBy(x => x.CategoryName).ToList();
        }

        /// <summary>
        /// Get categories which user has written review on
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<RCategory> GetCategoriesByUserReview(int userId)
        {
            return GetCategoriesByUserReview(userId, new MRContext());
        }
        /// <summary>
        /// Get categories which user has written review on
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<RCategory> GetCategoriesByUserReview(int userId, MRContext db)
        {
            List<RCategory> categories = GetSystemAndUserCategories(userId, db);
            categories = (from c in categories
                          join r in db.RReview
                          on c.RCategoryId equals r.RCategoryId
                          where r.CreateUserId == userId
                          select c).Distinct().ToList();
            return categories;

        }

        internal static RCategory InsertCategory(string categoryName, int userId)
        {
            return InsertCategory(categoryName, userId, new MRContext());
        }

        internal static RCategory InsertCategory(string categoryName, int userId, MRContext db)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;

            RCategory category = new RCategory();
            category.CategoryName = categoryName;
            category.CreateDate = DateTime.Now;
            category.CreateUserId = userId;
            category.LastModifiedDate = DateTime.Now;
            category.LastModifiedUserId = userId;
            db.Entry(category).State = System.Data.EntityState.Added;

            db.SaveChanges();
            return category;
        }


    }
}