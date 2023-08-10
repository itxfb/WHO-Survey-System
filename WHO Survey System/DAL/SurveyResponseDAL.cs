using Dapper;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WHO_Survey_System.BL;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.DAL
{
    public class SurveyResponseDAL
    {
        #region SurveyResponse
        public List<SurveyResponse> GetActiveSurveyResponseList(SqlConnection de, int start = 0, int length = 0, string sortColName = "", string sortDirection = "")
        {
            return de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse").OrderByDescending(a => a.Language == "English").ThenBy(a=>a.Id).DistinctBy(a=>a.Id).ToList();
            //return de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse, id, -1, '', 0," + length + "," + start + "," + sortColName + "," + sortDirection + "").ToList();
        }

        public List<SurveyResponse> GetAllSurveyResponseList(SqlConnection de)
        {
            return de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse, id, -1, ' where isActive <> -1'").ToList();
        }

        public SurveyResponse GetSurveyResponseById(int? Id, SqlConnection de)
        {
            //var test= de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse, Id," + Id + " ").FirstOrDefault();
            return de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse, Id," + Id + " ").FirstOrDefault();
        }

        public SurveyResponse GetSurveyResponseByEmail(string email, SqlConnection de)
        {
            return de.Query<SurveyResponse>("EXECUTE GetAllRecords SurveyResponse,Email," + email + " ").FirstOrDefault();
        }

        public bool AddSurveyResponse(SurveyResponse surveyResponse, SqlConnection de)
        {
            try
            {
                surveyResponse.IsActive = 1;
                surveyResponse.CreatedAt = DateTime.UtcNow;
                var getPropandVal = new UserBL().GetPropandVal(surveyResponse);
                var qr = "EXECUTE InsertOrUpdate 0,SurveyResponse," + getPropandVal[0] + "," + getPropandVal[1] + "";
                var add = de.Query("EXECUTE InsertOrUpdate 0,SurveyResponse," + getPropandVal[0] + "," + getPropandVal[1] + "").First();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSurveyResponse(SurveyResponse surveyResponse, SqlConnection de)
        {
            try
            {
                var getPropandVal = new UserBL().GetUpdatePropandVal(surveyResponse);
                var update = de.Query("EXECUTE InsertOrUpdate " + surveyResponse.Id + ",SurveyResponse,'" + getPropandVal + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public long SurveyResponsesCount(SqlConnection de)
        {
            try
            {
                var result = de.Query("SELECT COUNT(Id) as [Count] FROM SurveyResponse Where IsActive=1").SingleOrDefault();
                //dynamic result = de.Query("SELECT COUNT(Id) as [Count] FROM SurveyResponse Where IsActive=1").SingleOrDefault();
                
                return result.Count;
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