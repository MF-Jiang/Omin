using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    BoxCollider2D omniDetector; // 对omniDetector的引用
    Transform barrelTransform;  // 对炮管的transform组件引用
    BoxCollider2D barrelCollider;   // 对炮管的collider引用
    BoxCollider2D baseCollider; // 对底座collider的引用
    public CannonState state = CannonState.Enemy;  // 大炮默认初始为敌对状态

    public GameObject bullet;  // 对bullet的引用
    public float bulletSpeed; // bullet速度
    public float fireTime;  // 决定多久发射一次炮弹，单位为秒
    float timeAtLastPeroid; // fixed update的上一个周期开始的时间

    public float omniFireSpeed; // omni被发射时的初速度

    PlayerController playerController;  // 对玩家控制器的引用

    // Start is called before the first frame update
    void Start()
    {
        barrelTransform = transform.Find("Barrel");
        barrelCollider = barrelTransform.GetComponent<BoxCollider2D>();
        baseCollider = transform.Find("Base").GetComponent<BoxCollider2D>();
        timeAtLastPeroid = Time.time;   // 初始化计时器
        omniDetector = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == CannonState.Ally || state == CannonState.Ally_Firing)
        {
            allyStateBehaviourRunnable();
        }
    }

    private void FixedUpdate()
    {
        if (state == CannonState.Enemy)
        {
            enemyStateBehaviourRunnable();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 若omni碰撞到触发器，则显示进入大炮的UI
        if (collision.CompareTag("Player") && state == CannonState.Ally)
        {
            if (Input.GetKey(KeyCode.E))
            {
                // 按E进入大炮
                playerController = collision.GetComponent<PlayerController>();
                playerController.enterCannon(this);
                state = CannonState.Ally_Firing;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 该函数包含了omni飞出cannon之后重启collider的代码
        if (collision.CompareTag("Player") && state == CannonState.Ally && barrelCollider.enabled == false)
        {
            barrelCollider.enabled = true;
            baseCollider.enabled = true;    // 重启大炮的collider
        }
    }

    // 大炮作为敌方单位时的行为runnable函数
    void enemyStateBehaviourRunnable()
    {
        Quaternion direction = barrelTransform.rotation;   // 炮弹发射方向
        if(Time.time - timeAtLastPeroid > fireTime)
        {
            // 定时发射炮弹
            fireBullet();
            timeAtLastPeroid = Time.time;
        }
    }

    void allyStateBehaviourRunnable()
    {
        if (state == CannonState.Ally_Firing)
        {
            // 先瞄准
            playerAim();
            // 发射代码
            if (Input.GetKey(KeyCode.Space))
            {
                // 发射出omni并解除对playerController的引用
                baseCollider.enabled = false;
                barrelCollider.enabled = false; // 禁用barrel和base的碰撞体以避免干扰omni
                playerController.transform.position = new Vector3(barrelTransform.position.x, barrelTransform.position.y, playerController.transform.position.z);
                playerController.fireFromCannon(barrelTransform.rotation.z, omniFireSpeed); // 将omni移到发射位置上并发射出去
                playerController = null;
                // 回到ally状态
                state = CannonState.Ally;
            }
        }
    }

    void playerAim()
    {
        // 玩家操作大炮瞄准的函数
        // 获取鼠标坐标并转换成世界坐标，这部分代码是直接搬的playercontroller里的
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // 因为是2D，用不到z轴。使将z轴的值为0，这样鼠标的坐标就在(x,y)平面上了
        m_mousePosition.z = 0;
        Vector3 cannonPos = this.transform.position;    // 获取自身位置
        Vector2 targetDir = m_mousePosition - cannonPos;    // 计算目标角度
        float targetAngle = Vector2.Angle(targetDir, Vector3.up);   // 计算鼠标位置与对象位置角度
        if(m_mousePosition.x > cannonPos.x)
        {
            targetAngle = -targetAngle; // 若鼠标在大炮右边则还需要对角度反向
        }
        barrelTransform.transform.eulerAngles = new Vector3(0, 0, targetAngle - 90);    // 旋转使大炮对准相应角度
    }

    void stateChange()
    {
        state = CannonState.Ally;   // 切换机关形态，其他切换时需要执行的动作也应放在该函数中
    }

    void fireBullet()
    {
        GameObject newBullet = Instantiate(bullet) as GameObject;
        newBullet.transform.position = barrelTransform.position;
        newBullet.transform.rotation = barrelTransform.rotation;   // 设定炮弹朝向角度
        newBullet.GetComponent<TestBulletBehaviour>().speed = bulletSpeed;   // 设定炮弹速度
    }

    public enum CannonState
    {
        Enemy,
        Ally,
        Ally_Firing,
    }
}
