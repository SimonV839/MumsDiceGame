using System;
using System.Collections.Generic;

namespace EfDbFirstConsole.Models;

public partial class GameRequest
{
    public int GameRequestId { get; set; }

    public int? GameUserId { get; set; }

    public string? OpponentName { get; set; }

    public virtual GameUser? GameUser { get; set; }

    public virtual ICollection<GameUser> GameUsers { get; set; } = new List<GameUser>();
}
