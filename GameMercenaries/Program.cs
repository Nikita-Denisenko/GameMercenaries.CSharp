using GameMercenaries.gameManagement;

var itemTypes = GameData.Items.Select(item => item.GetType()).ToArray();
var units = GameData.Units.Select(unit => unit.Name).ToArray();
var locations = GameData.Locations.Select(l => l.Name).ToArray();

foreach (var item in itemTypes)
{
    Console.WriteLine(item);
}

foreach (var unit in units)
{
    Console.WriteLine(unit);
}

foreach (var location in locations)
{
    Console.WriteLine(location);
}


