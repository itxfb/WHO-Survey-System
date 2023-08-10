using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WHO_Survey_System.DAL;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.BL
{
    public class CompanyBL
    {
        #region Company
        public List<Company> GetActiveCompanyList(SqlConnection de)
        {
            return new CompanyDAL().GetActiveCompanyList(de);
        }

        public List<Company> GetAllCompanysList(SqlConnection de)
        {
            return new CompanyDAL().GetAllCompanyList(de);
        }

        public Company GetCompanyById(int _Id, SqlConnection de)
        {
            return new CompanyDAL().GetCompanyById(_Id, de);
        }

        public bool AddCompany(Company _Company, SqlConnection de)
        {
            if (_Company.CompanyName == "" || _Company.CompanyUrl == "" || _Company.TotalCompanySurvey == null)
            {
                return false;
            }
            else
            {
                return new CompanyDAL().AddCompany(_Company, de);
            }
        }

        public bool UpdateCompany(Company _Company, SqlConnection de)
        {
            if (_Company.CompanyName == "" || _Company.CompanyUrl == "" || _Company.TotalCompanySurvey == null)
            {
                return false;
            }
            else
            {
                return new CompanyDAL().UpdateCompany(_Company, de);
            }
        }

        public Company GetCompanyByName(string name, SqlConnection de)
        {
            return new CompanyDAL().GetCompanyByName(name, de);
        }


        public List<string> GetPropandVal(object obj)
        {
            return new CompanyDAL().GetPropandVal(obj);
        }

        public string GetUpdatePropandVal(object obj)
        {
            return new CompanyDAL().GetUpdatePropandVal(obj);
        }

        #endregion
    }
}