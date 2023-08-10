using WHO_Survey_System.DAL;
using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WHO_Survey_System.BL
{
    public class SurveyResponseBL
    {
        #region SurveyResponse
        public List<SurveyResponse> GetActiveSurveyResponseList(SqlConnection de, int start = 0, int length = 0, string sortColName = "", string sortDirection = "")
        {
            return new SurveyResponseDAL().GetActiveSurveyResponseList(de);
            //return new SurveyResponseDAL().GetActiveSurveyResponseList(de, start, length, sortColName, sortDirection);
        }

        public List<SurveyResponse> GetAllSurveyResponseList(SqlConnection de)
        {
            return new SurveyResponseDAL().GetAllSurveyResponseList(de);
        }

        public SurveyResponse GetSurveyResponseById(int? _Id, SqlConnection de)
        {
            return new SurveyResponseDAL().GetSurveyResponseById(_Id, de);
        }

        public SurveyResponse GetSurveyResponseByEmail(string Email, SqlConnection de)
        {
            return new SurveyResponseDAL().GetSurveyResponseByEmail(Email, de);
        }

        public bool AddSurveyResponse(SurveyResponse surveyResponse, SqlConnection de)
        {
            //if (String.IsNullOrEmpty(surveyResponse.Gender) || String.IsNullOrEmpty(surveyResponse.Age) || String.IsNullOrEmpty(surveyResponse.Experience)
            //    || String.IsNullOrEmpty(surveyResponse.Work_Place) || String.IsNullOrEmpty(surveyResponse.Contract_Category)
            //    || String.IsNullOrEmpty(surveyResponse.Language) || String.IsNullOrEmpty(surveyResponse.Responses))
            //{
            //    return false;
            //}
            if (String.IsNullOrEmpty(surveyResponse.Gender) || String.IsNullOrEmpty(surveyResponse.Age) || String.IsNullOrEmpty(surveyResponse.Experience)

                || String.IsNullOrEmpty(surveyResponse.Language) || String.IsNullOrEmpty(surveyResponse.Responses))
            {
                return false;
            }
            else
            {
                return new SurveyResponseDAL().AddSurveyResponse(surveyResponse, de);
            }
        }

        public bool UpdateSurveyResponse(SurveyResponse surveyResponse, SqlConnection de)
        {
            if(surveyResponse!=null)
            {
                if (String.IsNullOrEmpty(surveyResponse.Gender) || String.IsNullOrEmpty(surveyResponse.Age) || String.IsNullOrEmpty(surveyResponse.Experience))
                //|| String.IsNullOrEmpty(surveyResponse.Work_Place) || String.IsNullOrEmpty(surveyResponse.Contract_Category)
                //|| String.IsNullOrEmpty(surveyResponse.Language) || String.IsNullOrEmpty(surveyResponse.Responses))
                {
                    return false;
                }
                else
                {
                    return new SurveyResponseDAL().UpdateSurveyResponse(surveyResponse, de);
                }
            }

            return false;
            
        }

        public long SurveyResponsesCount(SqlConnection de)
        {
            return new SurveyResponseDAL().SurveyResponsesCount(de);
        }

        #endregion
    }
}