using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ItemDataManager
{
    private const string ItemConfigPath = "item";

    private static ItemDataManager instance;
    public static ItemDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ItemDataManager();
            }
            return instance;
        }
    }

    public Dictionary<int, ItemData> ItemDic { get; private set; } = new Dictionary<int, ItemData>();

    private ItemDataManager()
    {
        LoadItemConfig();
    }

    public bool TryGetItem(int id, out ItemData itemData)
    {
        return ItemDic.TryGetValue(id, out itemData);
    }

    private void LoadItemConfig()
    {
        TextAsset config = Resources.Load<TextAsset>(ItemConfigPath);
        if (config == null)
        {
            Debug.LogWarning("Item config not found in Resources/item.xml, fallback data will be used.");
            AddFallbackPotion();
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(config.text);

        XmlElement rootEle = doc.DocumentElement;
        if (rootEle == null)
        {
            AddFallbackPotion();
            return;
        }

        foreach (XmlElement itemEle in rootEle.GetElementsByTagName("item"))
        {
            ItemData itemData = new ItemData();
            itemData.id = ReadInt(itemEle, "id", 0);
            itemData.name = ReadString(itemEle, "name", "Unknown Item");
            itemData.description = ReadString(itemEle, "description", string.Empty);
            itemData.type = ReadItemType(ReadString(itemEle, "type", "Consumable"));
            itemData.recoverHp = ReadInt(itemEle, "recoverHp", 0);

            if (itemData.id != 0)
            {
                ItemDic[itemData.id] = itemData;
            }
        }

        if (ItemDic.Count == 0)
        {
            AddFallbackPotion();
        }
    }

    private string ReadString(XmlElement element, string tagName, string defaultValue)
    {
        XmlNodeList nodes = element.GetElementsByTagName(tagName);
        if (nodes.Count == 0)
        {
            return defaultValue;
        }

        return nodes[0].InnerText;
    }

    private int ReadInt(XmlElement element, string tagName, int defaultValue)
    {
        string value = ReadString(element, tagName, string.Empty);
        if (int.TryParse(value, out int result))
        {
            return result;
        }

        return defaultValue;
    }

    private ItemType ReadItemType(string value)
    {
        if (System.Enum.TryParse(value, true, out ItemType itemType))
        {
            return itemType;
        }

        return ItemType.Consumable;
    }

    private void AddFallbackPotion()
    {
        ItemDic[1001] = new ItemData
        {
            id = 1001,
            name = "Small Health Potion",
            description = "Recover 10 HP.",
            type = ItemType.Consumable,
            recoverHp = 10
        };
    }
}
