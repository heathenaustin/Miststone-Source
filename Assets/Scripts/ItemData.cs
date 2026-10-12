public enum ItemType
{
    Consumable
}

public class ItemData
{
    public int id;
    public string name;
    public string description;
    public ItemType type;
    public int recoverHp;
}
