using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.DAL
{
    public class CompanyDAL
    {
        #region Company
        public List<Company> GetActiveCompanyList(SqlConnection de)
        {

            return de.Query<Company>("EXECUTE GetAllRecords Company").ToList();
        }

        public List<Company> GetAllCompanyList(SqlConnection de)
        {
            return de.Query<Company>("EXECUTE GetAllRecords Company, id, -1, ' where isActive <> -1'").ToList();
        }

        public Company GetCompanyById(int Id, SqlConnection de)
        {
            return de.Query<Company>("EXECUTE GetAllRecords Company, Id," + Id + " ").FirstOrDefault();
        }

        public bool AddCompany(Company _Company, SqlConnection de)
        {
            try
            {
                var getPropandVal = GetPropandVal(_Company);
                var add = de.Query("EXECUTE InsertOrUpdate 0,Company," + getPropandVal[0] + "," + getPropandVal[1] + "").First();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCompany(Company _Company, SqlConnection de)
        {
            try
            {
                var getPropandVal = GetUpdatePropandVal(_Company);
                var update = de.Query("EXECUTE InsertOrUpdate " + _Company.Id + ",Company,'" + getPropandVal + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Company GetCompanyByName(string name, SqlConnection de)
        {
            try
            {
                //var testcompany= de.Query<Company>("EXECUTE GetAllRecords Company").ToList();
                return de.Query<Company>("EXECUTE GetAllRecords Company, CompanyName,'''" + name + "'''").FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<string> GetPropandVal(object obj)
        {
            try
            {
                var propAndval = new List<string>();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                var prop = new List<string>();
                var val = new List<string>();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj) != null && property.GetType() != typeof(object))
                    {
                        prop.Add(property.Name);
                        val.Add("''" + property.GetValue(obj).ToString() + "''");
                    }
                }
                prop = prop.Skip(1).ToList();
                val = val.Skip(1).ToList();
                propAndval.Add("'" + String.Join(",", prop) + "'");
                propAndval.Add("'" + String.Join(",", val) + "'");
                return propAndval;
            }
            catch
            {
                return null;
            }
        }
        public string GetUpdatePropandVal(object obj)
        {
            try
            {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                var prop = new List<string>();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj) != null && property.GetType() != typeof(object))
                    {
                        prop.Add(property.Name + " = ''" + property.GetValue(obj).ToString() + "''");
                    }
                }
                prop = prop.Skip(1).ToList();
                var getcolandval = String.Join(",", prop);
                return getcolandval;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}