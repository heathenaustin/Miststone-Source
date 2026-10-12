using UnityEngine;

public class HpManager : MonoBehaviour
{
    //关联HpText预制体
    public GameObject HpTextPre;

    public void ShowText(string text)
    {
        //实例化文字预制体
        GameObject go = Instantiate(HpTextPre, transform);
        //设置文字内容
        go.GetComponent<HpControl>().SetText(text);
    }

    void Update()
    {
        //面向摄影机
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
