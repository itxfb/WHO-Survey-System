using WHO_Survey_System.DAL;
using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WHO_Survey_System.BL
{
    public class SurveyAccessBL
    {
        #region SurveyAccess
        public List<SurveyAccessCredential> GetActiveSurveyAccessList(SqlConnection de)
        {
            return new SurveyAccessDAL().GetActiveSurveyAccessList(de);
        }

        public List<SurveyAccessCredential> GetAllSurveyAccessList(SqlConnection de)
        {
            return new SurveyAccessDAL().GetAllSurveyAccessList(de);
        }

        public SurveyAccessCredential GetSurveyAccessById(int _Id, SqlConnection de)
        {
            return new SurveyAccessDAL().GetSurveyAccessById(_Id, de);
        }

        public List<SurveyAccessCredential> GetSurveyAccessByCompanyId(int _Id, SqlConnection de)
        {
            return new SurveyAccessDAL().GetSurveyAccessByCompanyId(_Id, de);
        }

        public bool AddSurveyAccess(SurveyAccessCredential _sac, SqlConnection de)
        {
            if (String.IsNullOrEmpty(_sac.Email) || String.IsNullOrEmpty(_sac.Passcode))
            {
                return false;
            }
            else
            {
                return new SurveyAccessDAL().AddSurveyAccess(_sac, de);
            }
        }

        public bool UpdateSurveyAccess(SurveyAccessCredential _sac, SqlConnection de)
        {
            if (String.IsNullOrEmpty(_sac.Email) || String.IsNullOrEmpty(_sac.Passcode))
            {
                return false;
            }
            else
            {
                return new SurveyAccessDAL().UpdateSurveyAccess(_sac, de);
            }
        }

        public SurveyAccessCredential GetSurveyAccessByEmail(string email, SqlConnection de)
        {
            return new SurveyAccessDAL().GetSurveyAccessByEmail( email,  de);
        }

        public long SurveyAttemptedCount(SqlConnection de)
        {
            return new SurveyAccessDAL().SurveyAttemptedCount(de);
        } 
        public long SurveySubmittedCount(SqlConnection de)
        {
            return new SurveyAccessDAL().SurveySubmittedCount(de);
        }
        #endregion
    }
}