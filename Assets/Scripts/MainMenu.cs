using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject menuPanel;
    public GameObject loadingPanel;

    [Header("组件")]
    public Slider progressSlider;
    public Text progressText;

    public void StartGame()
    {
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        menuPanel.SetActive(false);
        loadingPanel.SetActive(true);

        // 1. 开始加载，并禁止自动跳转
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scene_A");
        operation.allowSceneActivation = false;

        float currentProgress = 0f;

        // 2. 【核心修改】这里是“造假”逻辑
        // 只要进度条没走满 (currentProgress < 1)，或者后台没加载完，就一直循环
        while (currentProgress < 1f || operation.progress < 0.9f)
        {
            // 真实的加载进度 (0 ~ 0.9)
            float realProgress = operation.progress;

            // 如果后台其实已经加载完了 (realProgress >= 0.9)，我们就假装目标是 1.0
            if (realProgress >= 0.9f)
            {
                realProgress = 1f;
            }

            // 【关键点】强制限制速度！
            // Time.deltaTime * 0.5f 意味着：每秒只走 50%。
            // 也就是不管电脑多快，这个进度条至少要走 2秒钟 (1 / 0.5 = 2) 才能走完。
            // 如果你想更慢，把 0.5f 改成 0.2f；想快点，改成 1.0f。
            currentProgress = Mathf.MoveTowards(currentProgress, realProgress, 0.5f * Time.deltaTime);

            // 更新 UI
            if (progressSlider != null) progressSlider.value = currentProgress;
            if (progressText != null) progressText.text = (int)(currentProgress * 100) + "%";

            yield return null;
        }

        // 3. 只有当进度条慢慢悠悠走到了 1，我们才放行
        // 再停顿 0.2 秒给玩家反应时间
        yield return new WaitForSeconds(0.2f);

        operation.allowSceneActivation = true;
    }
}
