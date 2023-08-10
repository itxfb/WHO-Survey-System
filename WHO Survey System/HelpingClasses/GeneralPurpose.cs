using WHO_Survey_System.BL;
using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WHO_Survey_System.HelpingClasses
{
    public class GeneralPurpose
    {
        SqlConnection de = new SqlConnection();
        DatabaseEntities db = new DatabaseEntities();

        public GeneralPurpose()
        {
            
            this.de = new SqlConnection(db.Database.Connection.ConnectionString);
        }




        public User ValidateLoggedinUser()
        {
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal; // Get the claims values
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            User loggedInUser = new UserBL().GetUserById(Convert.ToInt32(userId), de);
            return loggedInUser;
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public bool ValidateEmail(string email, int id = -1)
        {
            int emailCount = 0;
            if (id != -1)
            {
                emailCount = new UserBL().GetActiveUserList(de).Where(x => x.Email.ToLower() == email.ToLower() && x.Id != id && x.IsActive != 0).Count();
            }
            else
            {
                emailCount = new UserBL().GetActiveUserList(de).Where(x => x.Email.ToLower() == email.ToLower() && x.IsActive != 0).Count();
            }
            if (emailCount > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string GetUniqueKeyOriginal_BIASED(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        //public bool ValidateSurvey(string Title, int id = -1)
        //{
        //    int Count = 0;
        //    if (id != -1)
        //    {
        //        Count = new SurveyBL().GetActiveSurveyList(de).Where(x => x.Title.ToLower() == Title.ToLower() && x.Id != id && x.IsActive != 0).Count();
        //    }
        //    else
        //    {
        //        Count = new SurveyBL().GetActiveSurveyList(de).Where(x => x.Title.ToLower() == Title.ToLower() && x.IsActive != 0).Count();
        //    }
        //    if (Count > 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public bool ValidateCategory(string Title, int id = -1)
        //{
        //    int Count = 0;
        //    if (id != -1)
        //    {
        //        Count = new CategoryBL().GetActiveCategoryList(de).Where(x => x.Title.ToLower() == Title.ToLower() && x.Id != id && x.IsActive != 0).Count();
        //    }
        //    else
        //    {
        //        Count = new CategoryBL().GetActiveCategoryList(de).Where(x => x.Title.ToLower() == Title.ToLower() && x.IsActive != 0).Count();
        //    }
        //    if (Count > 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public bool ValidateScenario(string Question, int id = -1)
        //{
        //    int Count = 0;
        //    if (id != -1)
        //    {
        //        Count = new ScenarioBL().GetActiveScenarioList(de).Where(x => x.Question.ToLower() == Question.ToLower() && x.Id != id && x.IsActive != 0).Count();
        //    }
        //    else
        //    {
        //        Count = new ScenarioBL().GetActiveScenarioList(de).Where(x => x.Question.ToLower() == Question.ToLower() && x.IsActive != 0).Count();
        //    }
        //    if (Count > 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
}