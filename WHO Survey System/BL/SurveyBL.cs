using WHO_Survey_System.DAL;
using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WHO_Survey_System.BL
{
    public class SurveyBL
    {
        #region Survey

        public CreateSurvey GetActiveSurvey(SqlConnection de)
        {
            return new SurveyDAL().GetActiveSurvey(de);
        }
        public List<CreateSurvey> GetActiveSurveyList(SqlConnection de)
        {
            return new SurveyDAL().GetActiveSurveyList(de);
        }

        public List<CreateSurvey> GetAllSurveysList(SqlConnection de)
        {
            return new SurveyDAL().GetAllSurveysList(de);
        }

        public List<CreateSurvey> GetAllSurveysByCompanyId(int? _Id, SqlConnection de)
        {
            return new SurveyDAL().GetSurveyByCompanyId(_Id, de);
        }

        public CreateSurvey GetSurveyById(int? _Id, SqlConnection de)
        {
            return new SurveyDAL().GetSurveyById(_Id, de);
        }

        public bool AddSurvey(CreateSurvey survey, SqlConnection de)
        {
            if (String.IsNullOrEmpty(survey.Description) ||
                String.IsNullOrEmpty(survey.Title) ||
                String.IsNullOrEmpty(survey.Category_Scenarios) || survey.CompanyId == null)
            {
                return false;
            }
            else
            {
                return new SurveyDAL().AddSurvey(survey, de);
            }
        }

        public bool UpdateSurvey(CreateSurvey survey, SqlConnection de)
        {
            if (String.IsNullOrEmpty(survey.Description) || 
                String.IsNullOrEmpty(survey.Title) || 
                String.IsNullOrEmpty(survey.Category_Scenarios) || survey.CompanyId == null)
            {
                return false;
            }
            else
            {
                return new SurveyDAL().UpdateSurvey(survey, de);
            }
        }

        public long SurveysCount(SqlConnection de)
        {
            return new SurveyDAL().SurveysCount(de);
        }

        #endregion
    }
}