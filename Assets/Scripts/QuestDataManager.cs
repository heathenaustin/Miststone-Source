using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class QuestData
{
    public int id;
    public string name;
    public string description;
    public int enemyid;
    public int count;
    public int currentCount;
    public int money;
}

public class QuestDataManager
{
    private const string QuestConfigPath = "quest";

    private static QuestDataManager instance;
    public static QuestDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new QuestDataManager();
            }
            return instance;
        }
    }

    public Dictionary<int, QuestData> QuestDic = new Dictionary<int, QuestData>();

    private QuestDataManager()
    {
        LoadQuestConfig();
    }

    public QuestData CreateRuntimeQuest(int id)
    {
        if (!QuestDic.TryGetValue(id, out QuestData source))
        {
            return null;
        }

        return new QuestData
        {
            id = source.id,
            name = source.name,
            description = source.description,
            enemyid = source.enemyid,
            count = source.count,
            currentCount = 0,
            money = source.money
        };
    }

    private void LoadQuestConfig()
    {
        TextAsset config = Resources.Load<TextAsset>(QuestConfigPath);
        if (config == null)
        {
            AddFallbackQuest();
            return;
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(config.text);

        XmlElement rootEle = doc.DocumentElement;
        if (rootEle == null)
        {
            AddFallbackQuest();
            return;
        }

        foreach (XmlElement questEle in rootEle.GetElementsByTagName("quest"))
        {
            QuestData questData = new QuestData();
            questData.id = ReadInt(questEle, "id", 0);
            questData.name = ReadString(questEle, "name", "Defeat Stone Golems");
            questData.description = ReadString(questEle, "description", "Stone golems are gathering outside the village. Help the villager defeat two of them.");
            questData.enemyid = ReadInt(questEle, "enemyid", 101);
            questData.count = ReadInt(questEle, "count", 2);
            questData.money = ReadInt(questEle, "money", 100);

            if (questData.id != 0)
            {
                QuestDic[questData.id] = questData;
            }
        }

        if (QuestDic.Count == 0)
        {
            AddFallbackQuest();
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

    private void AddFallbackQuest()
    {
        QuestDic[1001] = new QuestData
        {
            id = 1001,
            name = "Defeat Stone Golems",
            description = "Stone golems are gathering outside the village. Help the villager defeat two of them.",
            enemyid = 101,
            count = 2,
            currentCount = 0,
            money = 100
        };
    }
}
