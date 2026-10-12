using UnityEngine;

public class NPCControl : MonoBehaviour
{
    public string Name = "村民";
    public string Content = "村外石头人正在聚集，可以帮我击败两个石头人吗？";
    public int QuestID = 1001;
    public float InteractDistance = 4f;

    private Transform player;
    private bool isPlayerNear;

    void Start()
    {
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);
        bool near = distance < InteractDistance;

        if (near != isPlayerNear)
        {
            isPlayerNear = near;
            if (QuestUI.Instance != null)
            {
                QuestUI.Instance.ShowInteractHint(isPlayerNear);
            }
        }

        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            if (QuestUI.Instance != null)
            {
                QuestUI.Instance.ShowInteractHint(false);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(Name, Content, QuestID);
            }
        }
    }

    private void OnDisable()
    {
        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.ShowInteractHint(false);
        }
    }
}
