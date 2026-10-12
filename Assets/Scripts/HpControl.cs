using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpControl : MonoBehaviour
{
    //计时器
    private float timer = 0;
    
    //设置数字
    public void SetText(string text)
    {
        GetComponent<TMP_Text>().text = text;
    }


    // Update is called once per frame
    void Update()
    {
        //计时器时间增加
        timer += Time.deltaTime;
        //如果超过1秒
        if(timer > 1)
        {
            //销毁自身
            Destroy(gameObject);
        }
        //移动
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
