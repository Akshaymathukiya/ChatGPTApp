using ChatGPT.Entities.Models;
using ChatGPT.Entities.ViewModels;
using ChatGPT.Repository.Interface;
using ChatGPTApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using System.Diagnostics;

namespace ChatGPTApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _iUserRepo;
        public HomeController(IUserRepository userRepo)
        {
            _iUserRepo = userRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User model)
        {
            var newUser = _iUserRepo.VerifyUserLogin(model);
            if (newUser == null)
            {
                TempData["error"] = "Please Complete Your Registration";
                return View();
            }
            else
            {
                if (model.Email == newUser.Email && model.Password == newUser.Password)
                {
                    HttpContext.Session.SetString("userName", newUser.Firstname + " " + newUser.Lastname);
                    HttpContext.Session.SetInt32("userId", newUser.Id);

                    TempData["success"] = "Login Successfully";
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    TempData["error"] = "Mismatch email or password!!!";
                    return View();
                }
            }
        }

        public IActionResult logout_user()
        {
            //int userId = HttpContext.Session.GetInt32("userId") ?? 0;
            //HttpContext.SignOutAsync().Wait();
            HttpContext.Session.Clear();
            TempData["success"] = "Logout Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddUsers(ChatGptViewModel user_data)
        {
            var addUser = _iUserRepo.AddUserData(user_data);
            return Json(new { success = true, redirectUrl = "/Home/Index" });

        }

        public IActionResult Home()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Home(string userSearch)
        {
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;

            ChatGptViewModel chatGPTModel = new ChatGptViewModel();
            if(userSearch == null)
            {
                chatGPTModel.user_result = "How can I help you?";
                return View(chatGPTModel);
            }
            var openAi = new OpenAIAPI("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");
            var completions = openAi.Completions.CreateCompletionAsync(

            prompt: userSearch,
            model: "text-davinci-002",
            max_tokens: 4000,
            temperature: 0.5f
            );
            //string result = string.Empty;
            //foreach (var completer in completions.Result.Completions)
            //{
            //    result = result + " " + completer.Text.ToString();
            //}
            string result = completions.Result.Completions.ElementAt(0).Text;
            chatGPTModel.user_result = result;
            
            //store the user history
            _iUserRepo.user_history(user_id, userSearch, result);
            return View(chatGPTModel);
        }

        public IActionResult UserHistory()
        {
            ChatGptViewModel userHistory = new ChatGptViewModel();
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            userHistory.getHistory = _iUserRepo.getHistory(user_id);
            return View(userHistory);
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}




