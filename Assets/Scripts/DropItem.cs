using UnityEngine;

public class DropItem : MonoBehaviour
{
    public int ItemId = 1001;
    public int Count = 1;

    private bool picked;

    private void OnTriggerEnter(Collider other)
    {
        if (picked || !other.CompareTag("Player"))
        {
            return;
        }

        picked = true;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider itemCollider in colliders)
        {
            itemCollider.enabled = false;
        }

        InventoryManager.Instance.AddItem(ItemId, Count);
        Destroy(gameObject);
    }
}
