using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    private static InventoryManager instance;
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InventoryManager();
            }
            return instance;
        }
    }

    public event Action<int, int> OnItemCountChanged;

    private readonly Dictionary<int, int> itemCounts = new Dictionary<int, int>();

    private InventoryManager()
    {
    }

    public void AddItem(int itemId, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (!ItemDataManager.Instance.TryGetItem(itemId, out ItemData itemData))
        {
            Debug.LogWarning("Cannot add unknown item id: " + itemId);
            return;
        }

        int currentCount = GetCount(itemId);
        int newCount = currentCount + count;
        itemCounts[itemId] = newCount;
        Debug.Log("Get item: " + itemData.name + " x" + count);
        OnItemCountChanged?.Invoke(itemId, newCount);
    }

    public bool RemoveItem(int itemId, int count)
    {
        if (count <= 0 || GetCount(itemId) < count)
        {
            return false;
        }

        int newCount = itemCounts[itemId] - count;
        if (newCount == 0)
        {
            itemCounts.Remove(itemId);
        }
        else
        {
            itemCounts[itemId] = newCount;
        }

        OnItemCountChanged?.Invoke(itemId, newCount);
        return true;
    }

    public bool UseItem(int itemId, PlayerControl player)
    {
        if (player == null)
        {
            return false;
        }

        if (!ItemDataManager.Instance.TryGetItem(itemId, out ItemData itemData))
        {
            Debug.LogWarning("Cannot use unknown item id: " + itemId);
            return false;
        }

        if (itemData.type != ItemType.Consumable || itemData.recoverHp <= 0)
        {
            Debug.LogWarning("Item is not usable: " + itemData.name);
            return false;
        }

        if (GetCount(itemId) <= 0)
        {
            return false;
        }

        if (player.Hp >= player.MaxHp)
        {
            Debug.Log("Player HP is already full.");
            return false;
        }

        if (!RemoveItem(itemId, 1))
        {
            return false;
        }

        player.Hp = Mathf.Min(player.MaxHp, player.Hp + itemData.recoverHp);
        Debug.Log("Use item: " + itemData.name);
        return true;
    }

    public int GetCount(int itemId)
    {
        return itemCounts.TryGetValue(itemId, out int count) ? count : 0;
    }

    public IReadOnlyDictionary<int, int> GetAllItems()
    {
        return itemCounts;
    }
}
