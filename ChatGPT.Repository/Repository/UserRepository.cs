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

        public void user_history(int user_id, string que, string ans)
        {
            UserHistory AddUserData = new UserHistory();    
            AddUserData.UserId = user_id;
            AddUserData.Question = que;
            AddUserData.Answer = ans;
            AddUserData.CreatedAt = DateTime.Now;

            _db.UserHistories.Add(AddUserData);
            _db.SaveChanges();
        }

        public List<UserHistory> getHistory(int userId)
        {
            var userHistory = _db.UserHistories.Where(u=>u.UserId == userId).ToList();
            return userHistory;
        }
        public List<UserHistory> getTodaysHistory(int userId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime dateOnly = currentDate.Date;

            var todayHist = _db.UserHistories.Where(u=>u.UserId==userId && u.CreatedAt.Date == dateOnly).ToList();
            return todayHist;
        }
        public bool delete_history(int id)
        {
            var user_history = _db.UserHistories.FirstOrDefault(u=>u.Id == id);
            if (user_history != null)
            {
                _db.UserHistories.Remove(user_history);
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
