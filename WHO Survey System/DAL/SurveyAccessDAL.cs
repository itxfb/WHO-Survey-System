using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;
using WHO_Survey_System.BL;
using WHO_Survey_System.HelpingClasses;
using Microsoft.Ajax.Utilities;

namespace WHO_Survey_System.DAL
{
    public class SurveyAccessDAL
    {
        #region SurveyAccess
        public List<SurveyAccessCredential> GetActiveSurveyAccessList(SqlConnection de)
        {
            return de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential").ToList();
        }

        public List<SurveyAccessCredential> GetAllSurveyAccessList(SqlConnection de)
        {
            return de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential, id, -1, ' where isActive <> -1'").ToList();
        }

        public SurveyAccessCredential GetSurveyAccessById(int Id, SqlConnection de)
        {
            return de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential, Id," + Id + " ").FirstOrDefault();
        }

        public List<SurveyAccessCredential> GetSurveyAccessByCompanyId(int Id, SqlConnection de)
        {
            return de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential, CompanyId," + Id + " ").ToList();
        }

        public bool AddSurveyAccess(SurveyAccessCredential _sac, SqlConnection de)
        {
            try
            {
                _sac.IsActive = 1;
                _sac.CreatedAt = DateTime.UtcNow;
                var getPropandVal = new UserBL().GetPropandVal(_sac);
                var add = de.Query("EXECUTE InsertOrUpdate 0,SurveyAccessCredential," + getPropandVal[0] + "," + getPropandVal[1] + "").First();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSurveyAccess(SurveyAccessCredential _sac, SqlConnection de)
        {
            try
            {
                var getPropandVal = new UserBL().GetUpdatePropandVal(_sac);
                var update = de.Query("EXECUTE InsertOrUpdate " + _sac.Id + ",SurveyAccessCredential,'" + getPropandVal + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }

    
        public SurveyAccessCredential GetSurveyAccessByEmail(string email, SqlConnection de)
        {
            try
            {
                //var emailDecode = StringCipher.Base64Decode(email);
                var testt = de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential").ToList();

                var test = de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential").Where(x=>x.Email== email).FirstOrDefault();
                
                return test;

                //return de.Query<SurveyAccessCredential>("EXECUTE GetAllRecords SurveyAccessCredential, Email,'''" + emailDecode + "'''").FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public long SurveyAttemptedCount(SqlConnection de)
        {
            try
            {
                var test = GetActiveSurveyAccessList(de).Where(x=>x.IsVerify==1 && x.IsActive==1 && x.IsSubmit==1).ToList().Count();

                // var query = "SELECT DISTINCT COUNT(Email) as [Count] FROM SurveyAccessCredential Where IsActive=1 And IsVerify=1 AND (Email like '%" + StringCipher.Base64Encode("@who.") + "%' OR Email like '%" + StringCipher.Base64Encode("@paho.") + "%' )";
       //bbr   //var result = GetActiveSurveyAccessList(de).Where(a=>(StringCipher.Base64Decode(a.Email).ToLower().Contains("@who.") || StringCipher.Base64Decode(a.Email).ToLower().Contains("@paho.")) && a.IsVerify==1).DistinctBy(a => StringCipher.Base64Decode(a.Email)).Count();
                //dynamic result = de.Query("SELECT COUNT(Id) as [Count] FROM SurveyResponse Where IsActive=1").SingleOrDefault();

                return test;
                //return de.Query("SELECT COUNT(Id) FROM SurveyResponse Where IsActive=1").Count();
            }
            catch
            {
                return 0;
            }
        }
        public long SurveySubmittedCount(SqlConnection de)
        {
            try
            {
                var result = GetActiveSurveyAccessList(de).Where(a => (StringCipher.Base64Decode(a.Email).ToLower().Contains("@who.") || StringCipher.Base64Decode(a.Email).ToLower().Contains("@paho.")) && a.IsSubmit == 1 && a.IsVerify == 1).DistinctBy(a=>StringCipher.Base64Decode(a.Email)).Count();
                //dynamic result = de.Query("SELECT COUNT(Id) as [Count] FROM SurveyResponse Where IsActive=1").SingleOrDefault();

                return result;
                //return de.Query("SELECT COUNT(Id) FROM SurveyResponse Where IsActive=1").Count();
            }
            catch
            {
                return 0;
            }
        }


        #endregion
    }
}