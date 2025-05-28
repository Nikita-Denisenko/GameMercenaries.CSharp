namespace GameMercenaries.models.items;

public class Artefact(
    string id,
    string name,
    string itemType,
    int weight,
    string info
    ) : Item(id, name, itemType, weight, info);