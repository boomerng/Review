using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyR.Filters;
using System.Web.Mvc;

namespace MyR.Models
{
    public partial class AccountContext : DbContext
    {
        public AccountContext()
            : base("MainConnection")
        { }

        public IDbSet<RUser> RUser { get; set; }
        public IDbSet<Role> Role { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    #region Membership
    [Table("RUser")]
    [Bind(Exclude = "RUserId, CreateDate, LastModifiedDate")]
    public partial class RUser
    {
        public RUser()
        {
            Roles = new List<Role>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RUserId { get; set; }
        [Display(Name = "User Email")]
        [Email(ErrorMessage = "Invalid Email")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string RUserEmail { get; set; }
        [StringLength(128)]
        [Display(Name = "Security Question")]
        public string Question { get; set; }
        [StringLength(128)]
        [Display(Name = "Security Answer")]
        public string Answer { get; set; }
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [StringLength(10)]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [StringLength(20)]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage="Invalid Phone")]
        public string Phone { get; set; }
        [StringLength(512)]
        [Display(Name="Profile Photo")]
        public string PhotoPath { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }

        [InverseProperty("RUsers")]
        public virtual ICollection<Role> Roles { get; set; }
    }

    [Table("webpages_Roles")]
    public partial class Role
    {
        public Role()
        {
            RUsers = new List<RUser>();
        }

        [Key]
        public int RoleId { get; set; }
        [StringLength(256)]
        public string RoleName { get; set; }
        [InverseProperty("Roles")]
        public virtual ICollection<RUser> RUsers { get; set; }
    }
    #endregion
}
