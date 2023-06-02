using ChatGPT.Entities.Models;
using ChatGPT.Entities.ViewModels;
using ChatGPT.Repository.Interface;
using ChatGPTApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ChatGPTApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _iUserRepo;
        private readonly IConfiguration _confige;
        public HomeController(IUserRepository userRepo, IConfiguration confige)
        {
            _iUserRepo = userRepo;
            _confige = confige;    
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            //var token = HttpContext.Request.Cookies["token"]?.ToString();
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
                    TokenManager tokenManager = new TokenManager();
                    var jwtsetting = _confige.GetSection(nameof(TokenKeyViewModel)).Get<TokenKeyViewModel>();
                    var token = tokenManager.GenerateToken(jwtsetting, newUser);
                    HttpContext.Response.Cookies.Append("token", token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddDays(1) });
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
            HttpContext.Response.Cookies.Delete("token");
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

        [Authorize]
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

        //public class ChatCompletionResponse
        //{
        //    public Choice[] Choices { get; set; }
        //}

        //public class Choice
        //{
        //    public Message Message { get; set; }
        //}

        //public class Message
        //{
        //    [JsonProperty("resultobject")]
        //    public string ResultObject { get; set; }
        //}

        public async Task<string> GetChatCompletionAsync(string userSearch, string userAssistant)
        {
            var httpClient = new HttpClient();
            var apiUrl = "https://api.openai.com/v1/chat/completions";
            var apiKey = "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";

            var payload = new
            {
                messages = new[]
                {
                   // new { role = "system", content = userAssistant },
                    new { role = "user", content = userSearch },
                    new { role = "assistant", content = userAssistant }
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
        public async Task<IActionResult> Home(string userSearch, String userAssistant)
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
            var model = await GetChatCompletionAsync(userSearch, userAssistant);
            var a = model.ToString();

            var question = userSearch + " " + userAssistant;
            _iUserRepo.user_history(user_id, question, a);
            chatGPTModel.today_history = _iUserRepo.getTodaysHistory(user_id);

            return View(chatGPTModel);
        }

        public IActionResult UserHistory()
        {
            ChatGptViewModel userHistory = new ChatGptViewModel();
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            if (user_id == 0)
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

            return PartialView("_UserHistory", userHistory);
        }

        //=========================================== Document part ============================
        public IActionResult Documents()
        {
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;
            var docs =  _iUserRepo.uploded_docs(user_id);
            DocumentsViewModel documentsViewModel = new DocumentsViewModel();
            documentsViewModel.docs = docs;

            return View(documentsViewModel);
        }

        [HttpPost]
        public ActionResult Documents(List<IFormFile> files)
        {
            int user_id = HttpContext.Session.GetInt32("userId") ?? 0;

            try
            {
                // Check if any files were selected
                if (files == null || !files.Any())
                {
                    ModelState.AddModelError("", "No files selected.");
                    return View("Documents");
                }

                // Validate the number of files
                int maxFiles = 10;
                if (files.Count() > maxFiles)
                {
                    ModelState.AddModelError("", $"Maximum {maxFiles} file(s) can be imported.");
                    return View("Documents");
                }

                // Validate file size
                long maxFileSize = 10 * 1024 * 1024; // 10MB
                long totalSize = 0;
                foreach (var file in files)
                {
                    if (file.Length > maxFileSize)
                    {
                        ModelState.AddModelError("", $"File '{file.FileName}' exceeds the maximum allowed size of 10MB.");
                        return View("Documents");
                    }

                    totalSize += file.Length;
                }

                // Validate file types
                var allowedFileTypes = new[] { ".pdf", ".jpeg", ".jpg", ".png" };
                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedFileTypes.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", $"File '{file.FileName}' has an unsupported file type. Only PDF, JPEG, and PNG files are allowed.");
                        return View("Documents");
                    }
                }

                // Read file content and store in the database using base64 encoding
                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    using (var binaryReader = new MemoryStream())
                    {
                        file.CopyToAsync(binaryReader);
                        var fileData = binaryReader.ToArray();

                        //var base64FileData = Convert.ToBase64String(fileData);

                        // Save the file data to the database using the repository
                        var store_doc = _iUserRepo.store_doc(file.FileName, fileData, user_id, fileExtension);
                        
                    }
                }
                TempData["success"] = "Files uploaded Successfully";
                return RedirectToAction("Home");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                ModelState.AddModelError("", "An error occurred while uploading the files: " + ex.Message);
                return View("Documents");
            }
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




