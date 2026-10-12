using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    //敌人id
    public int ID = 101;
    //主角
    public PlayerControl player;
    //血量
    public int Hp = 100;
    //攻击力
    public int Attack = 20;
    //出生点位置
    private Vector3 position;
    //动画器组件
    private Animator animator;
    //攻击计时器
    private float timer = 1;
    //当前是否在攻击
    private bool isAttack = false;
    //血瓶掉落预制体
    public GameObject PotionPre;
    //掉落物品id
    public int DropItemId = 1001;
    //掉落数量
    public int DropItemCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        //获取主角脚本
        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.GetComponent<PlayerControl>();
        }
        //获取出生点位置
        position = transform.position;
        //获取动画器组件
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        //如果主角死亡或自己死亡，则停止一切动作
        if (player.Hp <= 0 || Hp <= 0)
        {
            //停止播放攻击与移动动画
            animator.SetBool("Run", false);
            animator.SetBool("Attack", false);
            return;
        }

        //获取与主角的距离
        float distance = Vector3.Distance(player.transform.position, transform.position);
        //如果在周围7m没发现主角
        if (distance > 7f)
        {
            //距离出生点超过1m
            if (Vector3.Distance(transform.position, position) > 1f)
            {
                //转向出生点
                transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
                //向前移动
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
                //播放移动画面
                animator.SetBool("Run", true);
            }
            else
            {
                //停止播放移动画面
                animator.SetBool("Run", false);
            }
        }
        else if (distance > 3f)
        {
            //如果距离玩家在3m到7m之间，则向玩家移动
            //转向玩家
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            //向前移动
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            //播放移动画面
            animator.SetBool("Run", true);
            //保证当前不在攻击状态
            isAttack = false;
            animator.SetBool("Attack", false);

        }
        else
        {
            //3m内停止移动，开始攻击
            //停止播放移动画面
            animator.SetBool("Run", false);
            //转向玩家
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            //攻击
            animator.SetBool("Attack", true);
            //如果不在攻击状态
            if (isAttack == false)
            {
                //设置为攻击状态
                isAttack = true;
                //将计数器重置为1
                timer = 1;
            }
            //计时器时间增加
            timer += Time.deltaTime;
            //这里不同于主角
            //使用计时器来计算什么时候打出伤害
            //攻击时间为2s，我们在一秒的时候打出伤害
            if (timer >= 2)
            {
                timer = 0;
                //打出伤害
                player.GetDamage(Attack);
            }
        }
    }

    public void GetDamage(int damage)
    {
        if (Hp > 0)
        {
            //弹出伤害值
            HpManager hpManager = GetComponentInChildren<HpManager>();
            if (hpManager != null)
            {
                hpManager.ShowText("-" + damage);
            }
            //减少血量
            Hp -= damage;
            //如果血量为0
            if (Hp <= 0)
            {
                DropPotion();
                //播放死亡画面
                animator.SetTrigger("Die");
                //给任务系统报告，击杀一个id为101的敌人
                QuestManager.Instance.AddEnemy(ID);
                //销毁自己
                Destroy(gameObject, 2f);
            }
        }
    }

    private void DropPotion()
    {
        if (PotionPre == null)
        {
            return;
        }

        GameObject dropGo = Instantiate(PotionPre, transform.position, transform.rotation);
        PotionControl[] potionControls = dropGo.GetComponentsInChildren<PotionControl>();
        foreach (PotionControl potionControl in potionControls)
        {
            potionControl.enabled = false;
        }

        DropItem dropItem = dropGo.GetComponent<DropItem>();
        if (dropItem == null)
        {
            dropItem = dropGo.AddComponent<DropItem>();
        }

        dropItem.ItemId = DropItemId;
        dropItem.Count = DropItemCount;
    }
}

