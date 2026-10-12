using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject InventoryPanel;
    public Button InventoryButton;
    public Button CloseButton;

    [Header("First Item Slot")]
    public Transform FirstSlot;
    public Image ItemIcon;
    public TMP_Text CountText;
    public Button UseButton;

    [Header("Data")]
    public PlayerControl Player;
    public Sprite PotionIcon;
    public int PotionItemId = 1001;

    private void Awake()
    {
        if (Player == null)
        {
            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null)
            {
                Player = playerGo.GetComponent<PlayerControl>();
            }
        }

        AutoBindSlotReferences();

        if (InventoryPanel != null)
        {
            InventoryPanel.SetActive(false);
        }

        if (InventoryButton != null)
        {
            InventoryButton.onClick.AddListener(TogglePanel);
        }

        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(HidePanel);
        }

        if (UseButton != null)
        {
            UseButton.onClick.AddListener(UsePotion);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TogglePanel();
        }
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemCountChanged += OnItemCountChanged;
        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemCountChanged -= OnItemCountChanged;
    }

    public void TogglePanel()
    {
        if (InventoryPanel == null)
        {
            return;
        }

        bool show = !InventoryPanel.activeSelf;
        InventoryPanel.SetActive(show);
        SetCursorState(show);
        Refresh();
    }

    public void HidePanel()
    {
        if (InventoryPanel != null)
        {
            InventoryPanel.SetActive(false);
        }

        SetCursorState(false);
    }

    public void UsePotion()
    {
        bool used = InventoryManager.Instance.UseItem(PotionItemId, Player);
        if (!used)
        {
            Debug.Log("Use potion failed. Check item count, player reference, and whether HP is already full.");
        }

        Refresh();
    }

    private void OnItemCountChanged(int itemId, int newCount)
    {
        if (itemId == PotionItemId)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        int count = InventoryManager.Instance.GetCount(PotionItemId);
        bool hasPotion = count > 0;

        if (ItemIcon != null)
        {
            if (PotionIcon != null)
            {
                ItemIcon.sprite = PotionIcon;
            }

            ItemIcon.enabled = hasPotion && ItemIcon.sprite != null;
        }

        if (CountText != null)
        {
            CountText.text = hasPotion ? "x" + count : string.Empty;
        }

        if (UseButton != null)
        {
            UseButton.interactable = true;
        }
    }

    private void SetCursorState(bool showCursor)
    {
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }

    private void AutoBindSlotReferences()
    {
        if (FirstSlot == null)
        {
            return;
        }

        if (ItemIcon == null)
        {
            Transform iconTrans = FirstSlot.Find("ItemIcon");
            if (iconTrans != null)
            {
                ItemIcon = iconTrans.GetComponent<Image>();
            }
        }

        if (CountText == null)
        {
            Transform countTrans = FirstSlot.Find("CountText");
            if (countTrans != null)
            {
                CountText = countTrans.GetComponent<TMP_Text>();
            }
        }

        if (UseButton == null)
        {
            UseButton = FirstSlot.GetComponent<Button>();
            if (UseButton == null)
            {
                UseButton = FirstSlot.gameObject.AddComponent<Button>();
            }
        }
    }
}
