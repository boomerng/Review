using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MyR.Filters;

namespace MyR.Models
{
    public partial class MRContext : DbContext
    {
        public MRContext()
            : base("MainConnection")
        { 

        }

        public IDbSet<RCategory> RCategory { get; set; }
        public IDbSet<RReview> RReview { get; set; }
    }

    [Table("RCategory")]
    [Bind(Exclude = "CreateDate, LastModifiedDate")]
    public partial class RCategory
    {
        public RCategory()
        {
            
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RCategoryId { get; set; }
        [Required]
        [Display(Name = "Category")]
        [StringLength(100, ErrorMessage = "Category name is way too long.")]
        public string CategoryName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }
        public int CreateUserId { get; set; }
        public int LastModifiedUserId { get; set; }

        public virtual ICollection<RReview> RReviews { get; set; }
    }

    [Table("RReview")]
    [Bind(Exclude = "CreateDate, LastModifiedDate")]
    public partial class RReview
    {
        public RReview()
        {
            
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RReviewId { get; set; }
        [Required]
        [NotEqualValue(0, ErrorMessage="Please select a category first")]
        [Display(Name = "Category")]
        public int RCategoryId { get; set; }
        [Required]
        [Display(Name = "Review on Item")]
        [StringLength(100, ErrorMessage="Item name is way too long.")]
        public string ItemName { get; set; }
        [Display(Name = "Brand")]
        [StringLength(100, ErrorMessage = "Brand name is way too long.")]
        public string BrandName { get; set; }
        [Required]
        [Display(Name = "Title")]
        [StringLength(255, ErrorMessage = "Title name is way too long.")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Content")]
        public string BodyContent { get; set; }
        [Display(Name="Rating")]
        public float Rating { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }
        public int CreateUserId { get; set; }
        public int LastModifiedUserId { get; set; }

        [ForeignKey("RCategoryId")]
        public virtual RCategory Category { get; set; }
    }
}