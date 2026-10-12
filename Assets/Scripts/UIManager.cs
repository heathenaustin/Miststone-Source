using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private GameObject dialog;
    private int questid;
    private Image hpbar;
    private PlayerControl player;

    void Awake()
    {
        Instance = this;
        HideStartupDialog();
    }

    void Start()
    {
        Instance = this;

        Transform head = transform.Find("Head");
        if (head != null)
        {
            Transform hpBarTrans = head.Find("HpBar");
            if (hpBarTrans != null)
            {
                hpbar = hpBarTrans.GetComponent<Image>();
            }
        }

        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.GetComponent<PlayerControl>();
        }

        Transform dialogTrans = FindChildByName(transform, "Dialog");
        if (dialogTrans != null)
        {
            dialog = dialogTrans.gameObject;
            dialog.SetActive(false);
        }
    }

    private void HideStartupDialog()
    {
        Transform dialogTrans = FindChildByName(transform, "Dialog");
        if (dialogTrans != null)
        {
            dialogTrans.gameObject.SetActive(false);
        }
    }

    private Transform FindChildByName(Transform root, string childName)
    {
        if (root.name == childName)
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform result = FindChildByName(child, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    void Update()
    {
        if (hpbar != null && player != null && player.MaxHp > 0)
        {
            hpbar.fillAmount = (float)player.Hp / player.MaxHp;
        }
    }

    public void Show(string name, string content, int id = -1)
    {
        if (dialog == null)
        {
            Debug.LogWarning("Dialog object was not found under Canvas.");
            return;
        }

        questid = id;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dialog.SetActive(true);

        SetText("NameText", name);

        QuestState state = QuestManager.Instance.GetQuestState(id);
        if (state == QuestState.InProgress)
        {
            SetText("ContentText", "任务已经接取。");
        }
        else if (state == QuestState.Completed)
        {
            SetText("ContentText", "任务已经完成。");
        }
        else
        {
            SetText("ContentText", content);
        }
    }

    public void AcceptButtonClick()
    {
        QuestManager.Instance.AddQuest(questid);

        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.Refresh();
        }

        HideDialogAndLockCursor();
    }

    public void CancelButtonClick()
    {
        HideDialogAndLockCursor();
    }

    private void HideDialogAndLockCursor()
    {
        if (dialog != null)
        {
            dialog.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetText(string childName, string value)
    {
        if (dialog == null)
        {
            return;
        }

        Transform textTrans = FindChildByName(dialog.transform, childName);
        if (textTrans == null)
        {
            return;
        }

        TMP_Text tmpText = textTrans.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = value;
            return;
        }

        Text legacyText = textTrans.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = value;
        }
    }
}
