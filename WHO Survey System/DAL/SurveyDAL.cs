using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WHO_Survey_System.BL;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.DAL
{
    public class SurveyDAL
    {
        #region Survey

        public CreateSurvey GetActiveSurvey(SqlConnection de)
        {
            //return de.CreateSurveys.Where(x => x.IsActive == 1).FirstOrDefault();
            return de.Query<CreateSurvey>("EXECUTE GetAllRecords CreateSurvey").FirstOrDefault();
        }

        public List<CreateSurvey> GetActiveSurveyList(SqlConnection de)
        {
            return de.Query<CreateSurvey>("EXECUTE GetAllRecords CreateSurvey").ToList();
        }
    
        public List<CreateSurvey> GetAllSurveysList(SqlConnection de)
        {
            return de.Query<CreateSurvey>("EXECUTE GetAllRecords CreateSurvey, id, -1, ' where isActive <> -1'").ToList();
        }

        public List<CreateSurvey> GetSurveyByCompanyId(int? Id, SqlConnection de)
        {
            return de.Query<CreateSurvey>("EXECUTE GetAllRecords CreateSurvey, CompanyId," + Id + " ").ToList();
        }

        public CreateSurvey GetSurveyById(int? Id, SqlConnection de)
        {
            return de.Query<CreateSurvey>("EXECUTE GetAllRecords CreateSurvey, Id," + Id + " ").FirstOrDefault();
        }

        public bool AddSurvey(CreateSurvey survey, SqlConnection de)
        {
            try
            {
                //if(survey.Title.Contains("'"))
                //{
                //    var input = survey.Title;
                //    var output = input.Replace("'", "#");
                //    survey.Title = output;
                //}

                
                
                var getPropandVal = new UserBL().GetPropandVal(survey);
                var add = de.Query("EXECUTE InsertOrUpdate 0,CreateSurvey," + getPropandVal[0] + "," + getPropandVal[1] + "").First();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSurvey(CreateSurvey survey, SqlConnection de)
        {
            try
            {
                var getPropandVal = new UserBL().GetUpdatePropandVal(survey);
                var update = de.Query("EXECUTE InsertOrUpdate " + survey.Id + ",CreateSurvey,'" + getPropandVal + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }


        public long SurveysCount(SqlConnection de)
        {
            try
            {
                var result = de.Query("SELECT COUNT(Id) as [Count] FROM CreateSurvey Where IsActive=1").SingleOrDefault();
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