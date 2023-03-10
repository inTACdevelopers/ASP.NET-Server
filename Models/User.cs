using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Company { get; set; }

    public string? About { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<Post> Posts { get; } = new List<Post>();


}
