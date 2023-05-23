using ChatGPT.Entities.Data;
using ChatGPT.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGPT.Entities.ViewModels;
using ChatGPT.Entities.Models;

namespace ChatGPT.Repository.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly ChatGptContext _db;

        public UserRepository(ChatGptContext DbContext)
        {
            _db = DbContext;
        }

        public bool AddUserData(ChatGptViewModel user_data)
        {
            var user = new User();
            if(user_data != null)
            {
                user.Firstname = user_data.FirstName;
                user.Lastname = user_data.LastName;
                user.Email = user_data.Email;
                user.Mobilenumber = user_data.MobileNumber;
                user.Password = user_data.Password;
                user.CreatedAt = DateTime.Now;

                _db.Users.Add(user);
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public User VerifyUserLogin(User user)
        {
            User newUser = _db.Users.Where(u => u.Email == user.Email).FirstOrDefault();
            if (newUser == null)
            {
                return null;
            }
            else
            {
                return newUser;
            }
        }
    }
}
