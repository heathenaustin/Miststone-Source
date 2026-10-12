using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotAccepted,
    InProgress,
    Completed
}

public class QuestManager
{
    private static QuestManager instance;
    public static QuestManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new QuestManager();
            }
            return instance;
        }
    }

    public event Action<int> OnQuestChanged;

    private readonly Dictionary<int, QuestData> activeQuests = new Dictionary<int, QuestData>();
    private readonly HashSet<int> completedQuestIds = new HashSet<int>();

    public bool HasQuest(int id)
    {
        return activeQuests.ContainsKey(id) || completedQuestIds.Contains(id);
    }

    public void AddQuest(int id)
    {
        if (HasQuest(id))
        {
            OnQuestChanged?.Invoke(id);
            return;
        }

        QuestData questData = QuestDataManager.Instance.CreateRuntimeQuest(id);
        if (questData == null)
        {
            Debug.LogWarning("Cannot accept unknown quest id: " + id);
            return;
        }

        activeQuests[id] = questData;
        OnQuestChanged?.Invoke(id);
    }

    public void AddEnemy(int enemyid)
    {
        List<int> changedQuestIds = new List<int>();

        foreach (QuestData questData in activeQuests.Values)
        {
            if (questData.enemyid != enemyid || questData.currentCount >= questData.count)
            {
                continue;
            }

            questData.currentCount++;
            changedQuestIds.Add(questData.id);

            if (questData.currentCount >= questData.count)
            {
                completedQuestIds.Add(questData.id);
                PlayCompleteEffect();
            }
        }

        foreach (int questId in changedQuestIds)
        {
            OnQuestChanged?.Invoke(questId);
        }
    }

    public QuestState GetQuestState(int id)
    {
        if (completedQuestIds.Contains(id))
        {
            return QuestState.Completed;
        }

        if (activeQuests.ContainsKey(id))
        {
            return QuestState.InProgress;
        }

        return QuestState.NotAccepted;
    }

    public int GetCurrentCount(int id)
    {
        return activeQuests.TryGetValue(id, out QuestData questData) ? questData.currentCount : 0;
    }

    public int GetTargetCount(int id)
    {
        if (activeQuests.TryGetValue(id, out QuestData questData))
        {
            return questData.count;
        }

        if (QuestDataManager.Instance.QuestDic.TryGetValue(id, out QuestData configQuest))
        {
            return configQuest.count;
        }

        return 0;
    }

    public QuestData GetQuestData(int id)
    {
        if (activeQuests.TryGetValue(id, out QuestData questData))
        {
            return questData;
        }

        QuestDataManager.Instance.QuestDic.TryGetValue(id, out QuestData configQuest);
        return configQuest;
    }

    private void PlayCompleteEffect()
    {
        GameObject effectPrefab = Resources.Load<GameObject>("fx_hr_arthur_pskill_03_2");
        GameObject player = GameObject.FindWithTag("Player");
        if (effectPrefab != null && player != null)
        {
            GameObject.Instantiate(effectPrefab, player.transform.position, player.transform.rotation);
        }
    }
}
