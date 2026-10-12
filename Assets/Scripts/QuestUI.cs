using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header("Panel")]
    public GameObject QuestPanel;
    public Button QuestButton;
    public Button CloseButton;

    [Header("Quest Text - TMP")]
    public TMP_Text NameText;
    public TMP_Text DescText;
    public TMP_Text ProgressText;
    public TMP_Text StateText;
    public TMP_Text EmptyText;
    public TMP_Text InteractHintText;

    [Header("Quest Text - Legacy")]
    public Text NameLegacyText;
    public Text DescLegacyText;
    public Text ProgressLegacyText;
    public Text StateLegacyText;
    public Text EmptyLegacyText;
    public Text InteractHintLegacyText;

    [Header("Objects")]
    public GameObject QuestContentBox;
    public GameObject InteractHint;

    [Header("Data")]
    public int QuestId = 1001;

    private const string NoQuestText = "当前没有任务";
    private const string TalkHintText = "按 F 进行对话";

    private void Awake()
    {
        Instance = this;

        if (QuestPanel != null)
        {
            QuestPanel.SetActive(false);
        }

        ShowInteractHint(false);

        if (QuestButton != null)
        {
            QuestButton.onClick.AddListener(TogglePanel);
        }

        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(HidePanel);
        }
    }

    private void OnEnable()
    {
        QuestManager.Instance.OnQuestChanged += OnQuestChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestChanged -= OnQuestChanged;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (QuestPanel == null)
        {
            return;
        }

        bool show = !QuestPanel.activeSelf;
        QuestPanel.SetActive(show);
        SetCursorState(show);
        Refresh();
    }

    public void HidePanel()
    {
        if (QuestPanel != null)
        {
            QuestPanel.SetActive(false);
        }

        SetCursorState(false);
    }

    public void ShowInteractHint(bool show)
    {
        if (InteractHint != null)
        {
            InteractHint.SetActive(show);
        }

        SetText(InteractHintText, InteractHintLegacyText, TalkHintText);
    }

    public void Refresh()
    {
        QuestData questData = QuestManager.Instance.GetQuestData(QuestId);
        if (questData == null)
        {
            return;
        }

        QuestState state = QuestManager.Instance.GetQuestState(QuestId);
        if (state == QuestState.NotAccepted)
        {
            ShowNoQuest();
            return;
        }

        ShowQuestContent();

        int currentCount = QuestManager.Instance.GetCurrentCount(QuestId);
        int targetCount = QuestManager.Instance.GetTargetCount(QuestId);

        if (state == QuestState.Completed)
        {
            currentCount = targetCount;
        }

        SetText(NameText, NameLegacyText, questData.name);
        SetText(DescText, DescLegacyText, questData.description);
        SetText(ProgressText, ProgressLegacyText, "进度：" + currentCount + " / " + targetCount);
        SetText(StateText, StateLegacyText, GetStateText(state));
    }

    private void ShowNoQuest()
    {
        if (QuestContentBox != null)
        {
            QuestContentBox.SetActive(true);
        }

        if (HasText(EmptyText, EmptyLegacyText))
        {
            SetTextActive(EmptyText, EmptyLegacyText, true);
            SetText(EmptyText, EmptyLegacyText, NoQuestText);
            ClearQuestTexts();
            return;
        }

        SetText(NameText, NameLegacyText, NoQuestText);
        SetText(DescText, DescLegacyText, string.Empty);
        SetText(ProgressText, ProgressLegacyText, string.Empty);
        SetText(StateText, StateLegacyText, string.Empty);
    }

    private void ShowQuestContent()
    {
        if (QuestContentBox != null)
        {
            QuestContentBox.SetActive(true);
        }

        SetTextActive(EmptyText, EmptyLegacyText, false);
    }

    private void ClearQuestTexts()
    {
        SetText(NameText, NameLegacyText, string.Empty);
        SetText(DescText, DescLegacyText, string.Empty);
        SetText(ProgressText, ProgressLegacyText, string.Empty);
        SetText(StateText, StateLegacyText, string.Empty);
    }

    private void OnQuestChanged(int questId)
    {
        if (questId == QuestId)
        {
            Refresh();
        }
    }

    private string GetStateText(QuestState state)
    {
        switch (state)
        {
            case QuestState.InProgress:
                return "进行中";
            case QuestState.Completed:
                return "已完成";
            default:
                return "未接取";
        }
    }

    private void SetText(TMP_Text tmpText, Text legacyText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }

        if (legacyText != null)
        {
            legacyText.text = value;
        }
    }

    private bool HasText(TMP_Text tmpText, Text legacyText)
    {
        return tmpText != null || legacyText != null;
    }

    private void SetTextActive(TMP_Text tmpText, Text legacyText, bool active)
    {
        if (tmpText != null)
        {
            tmpText.gameObject.SetActive(active);
        }

        if (legacyText != null)
        {
            legacyText.gameObject.SetActive(active);
        }
    }

    private void SetCursorState(bool showCursor)
    {
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }
}
