using ChatGPT.Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace ChatGPT.Entities.ViewModels
{
    public class DocumentsViewModel
    {
        public List<IFormFile> Files { get; set; }
        public List<Document> docs { get; set; }

    }
}
