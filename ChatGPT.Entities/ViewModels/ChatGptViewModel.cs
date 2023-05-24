using ChatGPT.Entities.Models;

namespace ChatGPT.Entities.ViewModels
{
    public class ChatGptViewModel
    {
        public String? user_result { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public String? Email { get; set; }
        public int MobileNumber { get; set; }
        public String? Password { get; set; }
        public List<UserHistory> getHistory { get; set;}

    }
}
