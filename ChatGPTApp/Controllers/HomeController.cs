using ChatGPT.Entities.Models;
using ChatGPT.Entities.ViewModels;
using ChatGPT.Repository.Interface;
using ChatGPTApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Diagnostics;
using System.Text;

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
            ChatGptViewModel user_history = new ChatGptViewModel();
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            if (user_id == 0)
            {
                return View("Index");
            }
            user_history.today_history = _iUserRepo.getTodaysHistory(user_id);
            return View("Home", user_history);
        }
        
        public class ChatCompletionResponse
        {
            public Choice[] Choices { get; set; }
        }

        public class Choice
        {
            public Message Message { get; set; }
        }

        public class Message
        {
            [JsonProperty("resultobject")]
            public string ResultObject { get; set; }
        }

        public async Task<string> GetChatCompletionAsync(string userSearch)
        {
            var httpClient = new HttpClient();
            var apiUrl = "https://api.openai.com/v1/chat/completions";
            var apiKey = "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";

            var payload = new
            {
                messages = new[]
                {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "user", content = userSearch }
        },
                model = "gpt-3.5-turbo"
                //max_tokens = 4000,
                //temperature = 0.5
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var resultObject = responseObject.choices[0].message.content;
            //var resultObject = responseObject?.Choices[0].message.content;
            //var resultObject = responseObject?.Choices?[0]?.Message?.ResultObject;

            // Handle the result object as per your application's requirements
            return resultObject;
        }

        [HttpPost]
        public async Task<IActionResult> Home(string userSearch)
            {
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            if (user_id == 0)
            {
                return View("Index");
            }
            ChatGptViewModel chatGPTModel = new ChatGptViewModel();
            if (userSearch == null)
            {
                chatGPTModel.user_result = "How can I help you?";
                return View(chatGPTModel);
            }
            //var openAi = new OpenAIAPI("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");

            //var completions = openAi.Completions.CreateCompletionAsync(new CompletionRequest
            //{
            //    Prompt = userSearch,
            //    Model = "text-davinci-003",
            //    MaxTokens = 50,
            //    Temperature = 0.7f
            //});
            //string result = string.Empty;
            //foreach (var completer in completions.Result.Completions)
            //{
            //    result = result + " " + completer.Text.ToString();
            //}
            ////string result = completions.Result.Completions.ElementAt(0).Text;
            //chatGPTModel.user_result = result;

            ////store the user history
            var model = await GetChatCompletionAsync(userSearch);
            var a = model.ToString();
            chatGPTModel.Question = userSearch;
            chatGPTModel.user_result = a;
            _iUserRepo.user_history(user_id, userSearch, a);
            chatGPTModel.today_history = _iUserRepo.getTodaysHistory(user_id);

            return View(chatGPTModel);
        }

        public IActionResult UserHistory()
        {
            ChatGptViewModel userHistory = new ChatGptViewModel();
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            if(user_id == 0)
            {
                return View("Index");
            }
            userHistory.getHistory = _iUserRepo.getHistory(user_id);
            return View(userHistory);
        }

        public IActionResult delete_history(int id)
        {
            ChatGptViewModel userHistory = new ChatGptViewModel();

            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            var delete_history = _iUserRepo.delete_history(id);
            userHistory.getHistory = _iUserRepo.getHistory(user_id);

            return PartialView("_UserHistory",userHistory);
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




