using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyR.Models;

namespace MyR.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Review");
        }

        #region Add/Edit Review
        public ActionResult Review(int? id)
        {
            bool isNew = !id.HasValue;
            ReviewModel model;
            model = new ReviewModel(User.Identity.Name);
            SetReviewModelViewBags(model, isNew);

            if (!isNew && model.Review == null)
            {
                ModelState.AddModelError("ReviewNotFound", "Requested review is not found.");
                return View(new ReviewModel(User.Identity.Name));
            }
            else
                return View(model);
        }

        [HttpPost]
        public ActionResult Review(string submitButton, ReviewModel model)
        {
            if (submitButton != Constants.SystemConstants.CancelButton)
            {
                if (!ModelState.IsValid)
                {
                    model = new ReviewModel(User.Identity.Name);
                    SetReviewModelViewBags(model, true);
                    return View(model);
                }
                RUser user = RUser.GetUserByEmail(User.Identity.Name);
                model.Review = RReview.SaveReview(model.Review.RReviewId, model.Review.RCategoryId, model.Review.ItemName,
                    model.Review.BrandName, model.Review.Rating, model.Review.Title, model.Review.BodyContent, user.RUserId, model.NewCategoryName);

            }
            return RedirectToAction("Index", "Home");
        }

        private void SetReviewModelViewBags(ReviewModel model, bool isNew)
        {
            if (isNew)
            {
                ViewBag.Title = "New Review";
                ViewBag.SubmitButton = "Add";
            }
            else
            {
                ViewBag.Title = "Edit Reivew: " + model.Review.Title;
                ViewBag.SubmitButton = "Update";
            }

            ViewBag.NewCategoryClass = model.Categories.Any() ? "Hide" : "";
            ViewBag.ShowCategoryCancel = !model.Categories.Any() ? "Hide" : "";
        }
        #endregion

    }
}
