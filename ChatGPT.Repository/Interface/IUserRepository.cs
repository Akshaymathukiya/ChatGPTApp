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


    }
}
