using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int Owner { get; set; }

    public DateTime CreationDate { get; set; }

    public double Weight { get; set; }

    public virtual User OwnerNavigation { get; set; } = null!;
}
