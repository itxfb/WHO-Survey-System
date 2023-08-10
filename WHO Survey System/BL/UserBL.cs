using WHO_Survey_System.DAL;
using WHO_Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WHO_Survey_System.BL
{
    public class UserBL
    {
        #region User
        public List<User> GetActiveUserList(SqlConnection de)
        {
            return new UserDAL().GetActiveUserList(de);
        }

        public List<User> GetAllUsersList(SqlConnection de)
        {
            return new UserDAL().GetAllUsersList(de);
        }

        public User GetUserById(int _Id, SqlConnection de)
        {
            return new UserDAL().GetUserById(_Id, de);
        }

        public bool AddUser(User _user, SqlConnection de)
        {
            if (_user.FirstName == "" || _user.LastName == "" || _user.Email == "" || _user.Password == "" || _user.FirstName == null || _user.LastName == null || _user.Email == null || _user.Password == null)
            {
                return false;
            }
            else
            {
                return new UserDAL().AddUser(_user, de);
            }
        }

        public bool UpdateUser(User _user, SqlConnection de)
        {
            if (_user.FirstName == "" || _user.LastName == "" || _user.Email == "" || _user.Password == "" || _user.FirstName == null || _user.LastName == null || _user.Email == null || _user.Password == null)
            {
                return false;
            }
            else
            {
                return new UserDAL().UpdateUser(_user, de);
            }
        }

        public User GetUserByEmail(string email, SqlConnection de)
        {
            return new UserDAL().GetUserByEmail(email, de);
        }


        public List<string> GetPropandVal(object obj){
            return new UserDAL().GetPropandVal(obj);  
        }

        public string GetUpdatePropandVal(object obj)
        {
            return new UserDAL().GetUpdatePropandVal(obj);
        }

        #endregion
    }
}