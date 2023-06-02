using System;
using System.Collections.Generic;

namespace ChatGPT.Entities.Models;

public partial class Document
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Document1 { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public virtual User? User { get; set; }
}
