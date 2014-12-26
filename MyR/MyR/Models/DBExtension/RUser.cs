using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace MyR.Models
{
    public partial class RUser
    {
        internal static RUser GetUserByEmail(string email)
        {
            using (AccountContext db = new AccountContext())
            {
                return db.RUser.Where(x => x.RUserEmail == email).SingleOrDefault();
            }
        }

        internal static RUser GetUserByEmail(string email, AccountContext db)
        {
            return db.RUser.Where(x => x.RUserEmail == email).SingleOrDefault();
        }

        internal static RUser GetUserByChangePasswordToken(string token)
        {
            using (AccountContext db = new AccountContext())
            {
                int userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
                return db.RUser.Where(x => x.RUserId == userId).SingleOrDefault();
            }
        }

        internal static RUser GetUserByChangePasswordToken(string token, AccountContext db)
        {
            int userId = WebSecurity.GetUserIdFromPasswordResetToken(token);
            return db.RUser.Where(x => x.RUserId == userId).SingleOrDefault();
        }

        internal static bool IsLocalUser(RUser user)
        {
            return string.IsNullOrEmpty(user.Question);
        }

        internal static bool SaveUser(RUser rUser, string question, string answer, string firstName,
            string middelName, string lastName, string gender, string phone, string photoPath)
        {
            bool result = false;
            using (AccountContext db = new AccountContext())
            {
                db.Entry(rUser).State = System.Data.EntityState.Modified;
                if (!string.IsNullOrEmpty(question) && question.Length > 0)
                    rUser.Question = question;
                if (!string.IsNullOrEmpty(answer) && answer.Length > 0)
                    rUser.Answer = answer;
                if (!string.IsNullOrEmpty(firstName) && firstName.Length > 0)
                    rUser.FirstName = firstName;
                if (!string.IsNullOrEmpty(middelName) && middelName.Length > 0)
                    rUser.MiddleName = middelName;
                if (!string.IsNullOrEmpty(lastName) && lastName.Length > 0)
                    rUser.LastName = lastName;
                rUser.Gender = gender;
                if (!string.IsNullOrEmpty(phone) && phone.Length > 0)
                    rUser.Phone = phone;
                if (!string.IsNullOrEmpty(photoPath) && photoPath.Length > 0)
                    rUser.PhotoPath = photoPath;
                rUser.LastModifiedDate = DateTime.Now;
                try
                {
                    db.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    //TODO: write error log
                    result = false;
                }
            }
            return result;
        }
    }
}