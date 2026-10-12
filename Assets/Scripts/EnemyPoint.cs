using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoint : MonoBehaviour
{

    //关联敌人预制件
    public GameObject EnemyPre;

    //敌人生成的数量
    public int Num = 3;

    //计时器
    private float timer;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        //计时器时间增加
        timer += Time.deltaTime;
        //2s检测一次
        if(timer > 2)
        {
            //重置计时器
            timer = 0;
            //查看有几个敌人
            int n = transform.childCount;
            if(n < Num)
            {
                //随机确定一个位置
                Vector3 v = transform.position;
                v.x += Random.Range(-5, 5);
                v.z += Random.Range(-5, 5);
                //随机确认一个旋转
                Quaternion q = Quaternion.Euler(0, Random.Range(0, 360), 0);
                //创建一个敌人
                GameObject go = GameObject.Instantiate(EnemyPre, v, q);
                //设置父子关系
                go.transform.SetParent(transform);
            }
        }
    }
}
