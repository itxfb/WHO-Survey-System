using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Reflection;
using Dapper;

namespace WHO_Survey_System.DAL
{
    public class UserDAL
    {
        #region User
        public List<User> GetActiveUserList(SqlConnection de)
        {
            return de.Query<User>("EXECUTE GetAllRecords Users").ToList();
        }

        public List<User> GetAllUsersList(SqlConnection de)
        {
            return de.Query<User>("EXECUTE GetAllRecords Users, id, -1, ' where isActive <> -1'").ToList();
        }

        public User GetUserById(int Id, SqlConnection de)
        {
            var test = de.Query<User>("EXECUTE GetAllRecords Users, Id," + Id + " ").FirstOrDefault();

            return de.Query<User>("EXECUTE GetAllRecords Users, Id," + Id + " ").FirstOrDefault();
        }

        public bool AddUser(User _user, SqlConnection de)
        {
            try
            {
                var getPropandVal = GetPropandVal(_user);
                var add = de.Query("EXECUTE InsertOrUpdate 0,Users," + getPropandVal[0] + "," + getPropandVal[1] + "").First();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(User _user, SqlConnection de)
        {
            try
            {
                var getPropandVal = GetUpdatePropandVal(_user);
                var update = de.Query("EXECUTE InsertOrUpdate " + _user.Id + ",Users,'" + getPropandVal + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public User GetUserByEmail(string email, SqlConnection de)
        {
            try
            {
                //var qr = "EXECUTE GetAllRecords Users, Email,'''" + email + "'''";
                var qr = de.Query<User>("EXECUTE GetAllRecords Users, Email,'''" + email + "'''").FirstOrDefault();
                return de.Query<User>("EXECUTE GetAllRecords Users, Email,'''" + email + "'''").FirstOrDefault();
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
                    if (property.GetValue(obj) != null && property.GetType() != typeof(object) )
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