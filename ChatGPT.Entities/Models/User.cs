using System;
using System.Collections.Generic;

namespace ChatGPT.Entities.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public long Mobilenumber { get; set; }

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();
}
