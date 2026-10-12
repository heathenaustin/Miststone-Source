using UnityEngine;

public class PotionControl : MonoBehaviour
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

        if (GetComponentInParent<DropItem>() != null)
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
