using ChatGPT.Entities.Models;
using ChatGPT.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT.Repository.Interface
{
    public interface IUserRepository
    {
        public bool AddUserData(ChatGptViewModel user_data);
        public User VerifyUserLogin(User user);
        public void user_history(int user_id, string que, string ans);
        public List<UserHistory> getHistory(int userId);
        public bool delete_history(int id);
        public List<UserHistory> getTodaysHistory(int userId);

    }
}
