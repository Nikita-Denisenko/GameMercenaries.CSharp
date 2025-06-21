using GameMercenaries.Constants;

namespace GameMercenaries.Models;

public class GameEvent
{
    public int Day { get; init; }
    public EventType Type { get; init; }
    public required string Message { get; init; }
}