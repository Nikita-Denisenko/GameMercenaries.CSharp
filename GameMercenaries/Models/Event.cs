using GameMercenaries.Constants;

namespace GameMercenaries.Models;

public class Event
{
    public int DayNumber { get; init; }
    public EventType Type { get; init; }
    public required string Message { get; init; }
}