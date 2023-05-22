using ChatGPTApp.Models;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using System.Diagnostics;

namespace ChatGPTApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userSearch)
        {
            ChatGPTModel chatGPTModel = new ChatGPTModel();
            var openAi = new OpenAIAPI("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");

            var completions = openAi.Completions.CreateCompletionAsync(
            prompt: userSearch,
            model: "text-davinci-002",
            max_tokens:4000,
            temperature: 0.5f
            );
            chatGPTModel.user_result = completions.Result.Completions.ElementAt(0).Text;
            return View(chatGPTModel);
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