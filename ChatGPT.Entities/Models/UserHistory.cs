using System;
using System.Collections.Generic;

namespace ChatGPT.Entities.Models;

public partial class UserHistory
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
