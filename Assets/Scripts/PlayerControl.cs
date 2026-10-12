using System.Collections.Generic;
using UnityEngine;

//主角状态枚举
public enum PlayerState
{
    idle,
    run,
    die,
    attack,
    attack2
}

public class PlayerControl : MonoBehaviour
{
    //主角状态
    private PlayerState state = PlayerState.idle;

    //刚体组件
    private Rigidbody rBody;

    //动画器组件
    private Animator animator;

    //最大血量
    public int MaxHp = 100;

    //血量
    public int Hp = 100;

    //特效数组
    private List<Transform> fxList;

    //特效预制体缓存
    private Dictionary<string, GameObject> fxPrefabCache;

    // Start is called before the first frame update
    void Start()
    {
        //获取刚体组件
        rBody = GetComponent<Rigidbody>();
        //获取动画器组件
        animator = GetComponent<Animator>();
        //隐藏鼠标指针
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //实例化数组
        fxList = new List<Transform>();
        fxPrefabCache = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //按下alt键
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            //显示鼠标
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Cursor.lockState == CursorLockMode.None;
        }
        //如果鼠标为呼出状态，则什么也不做
        if (Cursor.lockState == CursorLockMode.None)
        {
            return;
        }

        //判断状态
        switch (state)
        {
            case PlayerState.idle:
                //允许旋转
                Rotate();
                //允许移动
                Move();
                //攻击
                Attack();
                //播放站立动画
                animator.SetBool("Run", false);
                break;
            case PlayerState.run:
                //允许旋转
                Rotate();
                //允许移动
                Move();
                //攻击
                Attack();
                //播放移动画面
                animator.SetBool("Run", true);
                break;
            case PlayerState.die:
                break;
            case PlayerState.attack:
                break;
            case PlayerState.attack2:
                break;
        }

        UpdateFlyingEffects();
    }

    void UpdateFlyingEffects()
    {
        // 使用倒序循环，这样在删除元素时不会影响后续元素的索引，也不会报错
        for (int i = fxList.Count - 1; i >= 0; i--)
        {
            Transform trans = fxList[i];

            // 判空保护（防止特效物体已经被意外销毁）
            if (trans == null)
            {
                fxList.RemoveAt(i);
                continue;
            }

            // 特效移动
            trans.Translate(Vector3.forward * 20 * Time.deltaTime);

            // 判断周围有没有敌人
            Collider[] colliders = Physics.OverlapSphere(trans.position, 1f);

            foreach (Collider collider in colliders)
            {
                // 如果附近有敌人
                if (collider.CompareTag("Enemy"))
                {
                    // 敌人血量减少
                    var enemy = collider.GetComponent<EnemyControl>();
                    if (enemy != null) enemy.GetDamage(10);

                    // 爆炸特效
                    GameObject fxPre = LoadFxPrefab("Explosion");
                    if (fxPre != null)
                    {
                        GameObject go = Instantiate(fxPre, collider.transform.position, collider.transform.rotation);
                        Destroy(go, 2f);
                    }

                    // 销毁当前的飞行特效物体
                    Destroy(trans.gameObject);

                    // 从列表中移除该数据
                    fxList.RemoveAt(i);

                    break; // 击中一个敌人后，跳出碰撞检测循环，处理下一个特效
                }
            }
        }
    }

    //攻击
    void Attack()
    {
        //单击鼠标左键攻击
        if (Input.GetMouseButtonDown(0))
        {
            //播放攻击动画
            animator.SetTrigger("Attack");
            //攻击状态
            state = PlayerState.attack;
        }

        if (Input.GetMouseButtonDown(1))
        {
            //播放攻击动画
            animator.SetTrigger("Attack2");
            //攻击状态
            state = PlayerState.attack2;
        }
    }

    //主角的移动控制
    void Move()
    {
        // 1. 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. 只有在有输入时才修改水平速度，否则保持现有水平速度（或者做减速处理，这里先只改Bug）
        // 为了防止逻辑冲突，状态切换建议放在Update的Switch里，这里只负责物理

        // 计算目标水平速度向量 (使用 transform.forward 和 right 保证是相对于角色/相机的方向)
        // 假设向前速度是4，横移是2
        Vector3 targetMoveDir = (transform.forward * vertical * 4) + (transform.right * horizontal * 2);

        // 3. 【核心修复】保留刚体原本的 Y 轴速度！
        // 这样重力（Gravity）对 Y 轴施加的影响就被保留下来了
        rBody.velocity = new Vector3(targetMoveDir.x, rBody.velocity.y, targetMoveDir.z);

        // 4. 状态判断移交出去，或者在这里简单处理（为了不破坏你现有结构，暂时保留）
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // 只有当前是Idle或Run时才允许切换，防止打断攻击
            if (state == PlayerState.idle || state == PlayerState.run)
                state = PlayerState.run;
        }
        else
        {
            if (state == PlayerState.run)
                state = PlayerState.idle;
        }
    }

    //主角的旋转控制
    void Rotate()
    {
        //主角与摄像机同步旋转
        transform.rotation = Camera.main.transform.parent.rotation;
    }

    //结束攻击
    void AttackEnd()
    {
        //恢复为站立状态
        state = PlayerState.idle;
    }

    //受到攻击
    public void GetDamage(int damage)
    {
        //减少血量
        Hp -= damage;
        //如果血量为0
        if (Hp <= 0)
        {
            //变为死亡状态
            state = PlayerState.die;
            //播放一次死亡动画
            animator.SetTrigger("Die");
            //死亡3秒后复活
            Invoke("Revive", 3f);
        }
    }

    //复活
    public void Revive()
    {
        //如果是死亡状态
        if (state == PlayerState.die)
        {
            //血量恢复
            Hp = MaxHp;
            //复活
            animator.SetTrigger("Revive");
            //复活为站立状态
            state = PlayerState.idle;
        }
    }

    //对敌人造成伤害
    void Damage(int damage)
    {
        //获取3m内的物体
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        //遍历物体
        foreach (Collider collider in colliders)
        {
            //判断是否是敌人且在60°的攻击范围内
            if (collider.CompareTag("Enemy") && Vector3.Angle(collider.transform.position - transform.position, transform.forward) < 60)
            {
                //敌人收到伤害
                var enemy = collider.GetComponent<EnemyControl>();
                if (enemy != null) enemy.GetDamage(damage);
            }
        }
    }

    GameObject LoadFxPrefab(string name)
    {
        if (!fxPrefabCache.TryGetValue(name, out GameObject prefab))
        {
            prefab = Resources.Load<GameObject>(name);
            fxPrefabCache[name] = prefab;
        }
        return prefab;
    }

    Transform FX(string name, float desTime)
    {
        //加载特效预载体
        GameObject fxPre = LoadFxPrefab(name);
        if (fxPre == null)
        {
            Debug.LogWarning("特效预制体加载失败：" + name);
            return null;
        }

        //实例化特效
        GameObject go = Instantiate(fxPre, transform.position, transform.rotation);
        //删除特效物体
        Destroy(go, desTime);
        return go.transform;
    }

    //攻击1_1
    void Attack1_1()
    {
        //攻击
        Damage(20);
        //特效
        FX("fx_hr_arthur_attack_01_1", 0.5f);
    }

    //攻击1_2
    void Attack1_2()
    {
        //攻击
        Damage(20);
        //特效
        FX("fx_hr_arthur_attack_01_2", 0.5f);
        //添加能量火焰特效
        for (int i = 0; i < 5; i++)
        {
            //创建火焰特效
            Transform fire = FX("Magic fire pro red", 1f);
            if (fire == null)
            {
                continue;
            }
            //设置火焰特效的旋转
            fire.transform.rotation = transform.rotation;
            //设置不同旋转角度
            fire.transform.Rotate(fire.transform.up, 15 * i - 30);
            //添加到特效数组
            fxList.Add(fire);
        }
        Invoke("ClearFXList", 1f);
    }

    //清空特效数组
    void ClearFXList()
    {
        fxList.Clear();
    }

    //攻击2_0
    void Attack2_0()
    {
        //特效
        FX("fx_hr_arthur_pskill_03_1", 1f);
        //增加一个特效
        FX("RotatorPS2", 4f);
    }

    //攻击2_1
    void Attack2_1()
    {
        //攻击
        Damage(80);
        //特效
        FX("fx_hr_arthur_pskill_01", 1.8f);
    }

    //攻击2_2
    void Attack2_2()
    {
        //攻击
        Damage(20);
    }
}

