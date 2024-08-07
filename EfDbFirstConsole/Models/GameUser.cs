using System;
using System.Collections.Generic;

namespace EfDbFirstConsole.Models;

public partial class GameUser
{
    public int GameUserId { get; set; }

    public string? Name { get; set; }

    public int? GameRequestId { get; set; }

    public int? DiceGameId { get; set; }

    public virtual DiceGame? DiceGame { get; set; }

    public virtual ICollection<DiceGame> DiceGamePlayer1s { get; set; } = new List<DiceGame>();

    public virtual ICollection<DiceGame> DiceGamePlayer2s { get; set; } = new List<DiceGame>();

    public virtual GameRequest? GameRequest { get; set; }

    public virtual ICollection<GameRequest> GameRequests { get; set; } = new List<GameRequest>();
}
