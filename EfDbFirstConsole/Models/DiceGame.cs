using System;
using System.Collections.Generic;

namespace EfDbFirstConsole.Models;

public partial class DiceGame
{
    public int DiceGameId { get; set; }

    public int? Player1Id { get; set; }

    public int? Player2Id { get; set; }

    public bool? IsPlayer1Turn { get; set; }

    public bool? IsAborted { get; set; }

    public virtual ICollection<GameUser> GameUsers { get; set; } = new List<GameUser>();

    public virtual GameUser? Player1 { get; set; }

    public virtual GameUser? Player2 { get; set; }
}
