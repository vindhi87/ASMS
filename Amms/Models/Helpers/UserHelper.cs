using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amms.Models.Helpers
{
    public class UserHelper
    {
        public User GetUser(int userId)
        {
            return new User();
        }

        public List<User> GetAllUsers()
        {
            List<User> ListUser = new List<User>();
            User user;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {

                //var Users = dc.Users.Where( er => er.IsEmailVerified == false);
                var Users = dc.Users; // load all data from User table 
                foreach (var usr in Users)
                {
                    user = new User();
                    user.DateOfBirth = usr.DateOfBirth;
                    user.EmailID = usr.EmailID;
                    user.Employee = usr.Employee;
                    user.EmployeeID = usr.EmployeeID;
                    user.FirstName = usr.FirstName;
                    user.LastName = usr.LastName;
                    user.UserID = usr.UserID;
                    user.UserName = usr.UserName;

                    ListUser.Add(user);
                }


            }

            return ListUser;
        }

        public int AddUser(User user)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                try
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();
                    return 0;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }
    }
}