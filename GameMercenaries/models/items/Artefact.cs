namespace GameMercenaries.models.items;

public class Artefact(
    string id,
    string name,
    string itemType,
    int maxQuantity,
    int currentQuantity,
    int weight,
    string info
    ) : Item(id, name, itemType, maxQuantity, currentQuantity, weight, info);