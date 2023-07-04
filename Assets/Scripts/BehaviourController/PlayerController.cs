using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MoveableObject
{
    // 角色运动参数
    [Header ("=========Character motion parameters=========")]
    public float moveSpeed = 4;
    public float maxJumpHeight = 2;
    public int maxJumpTime = 2;
    float jumpInitVelocity;
    public int curJumpCount;
    public bool isGrounded = true;
    public JumpState jumpState = JumpState.Grounded;

    public bool couldDoubleJump = true;

    public float curSpriteSize; // 角色sprite目前的大小，default为1
    [SerializeField]private Rigidbody2D _BullectDetect_rb;

    // 角色状态相关量
    public OmniState initState = OmniState.Liquid;
    public OmniState currentState;
    public MoveableState moveableState = MoveableState.Moveable;

    // 角色生命值相关量
    public static float maxLife = 5.0f;
    public static float currentLife;
    // 收到的伤害;
    public float totalpoisonHurt = 0.0f;
    public bool hurt =false;
    // 对sprite其他组件的引用
    BoxCollider2D selfCollider;

    [Header ("====== Jump variable=========")]
    
    float hMove;
    bool jumpPressed;

    [Header("====== Shoot variable========")]
    //子弹的gameobject
    public GameObject bullet;
    private float loading = 0;
    public bool shootAction = false;

    //角色动画控制器
    private Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<BoxCollider2D>();   // 初始化rigidbody和collider引用

        // 初始化运动参数
        jumpInitVelocity = Mathf.Sqrt( 2 * maxJumpHeight * Mathf.Abs(GlobalPhysicParameter.gravity) ); // 根据设定参数计算起跳速度
        curJumpCount = maxJumpTime; // 初始化跳跃次数
        hMove = 0;
        jumpPressed = false;

        // 初始化生命值与状态
        currentLife = maxLife;
        currentState = initState;
    }

    // Update is called once per frame
    void Update()
    {
        // movements control
        if (moveableState == MoveableState.Moveable)
        {
            updateInput();
            updateHorizontalMove();
            CheckGrounded();
            updateJumping();    // 只有在允许运动时可以跳跃或移动
        }
        else if (moveableState == MoveableState.WaitingForMoveable)
        {
            resumeMoveableDetect();
        }
        
        Shoot();
        dieDetect();

        
        //SwitchAnimation() 预留一个上升到下落动画切换的函数
        //角色死亡
        if (currentLife <= Mathf.Epsilon) {
            Destroy(gameObject);
        }
    }

    //检测是不是在地面上
    void CheckGrounded() {
        isGrounded = selfCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isGrounded && rigidBody.velocity.y == 0) {
            curJumpCount = maxJumpTime;
        }
        //Debug.Log(isGrounded);
    }

    //建议带有刚体的物体的操作都放在fixedupdate中，防止卡顿
    void FixedUpdate()
    {

    }

    void updateInput()
    {
        hMove = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetButtonDown("Jump");
    }

    void updateHorizontalMove()
    {
        // get input, and then calculate the horizontal speed and movement
        rigidBody.velocity = new Vector2(hMove * moveSpeed, rigidBody.velocity.y);
    }

    void groundedDetect()
    {
        // 该函数检测sprite是否接触了地面/其他可落脚的物体

    }

    void updateJumping()
    {
        if (jumpPressed)
        {
            //Debug.Log("a");
            if (isGrounded || curJumpCount==2)
            {
                //Debug.Log("b");
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpInitVelocity);
                couldDoubleJump = true;
                curJumpCount-=1;
            }
            else
            {
                if (couldDoubleJump)
                {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpInitVelocity);
                    couldDoubleJump = false;
                }

            }
        
        
        }


/*        if (jumpPressed && curJumpCount > 0)
        {
            // if jump count > 0, jump for one time
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpInitVelocity);
            curJumpCount -= 1;
        }
        else if(rigidBody.velocity.y <= minYSpeed)
        {
            // reset jump count
            curJumpCount = maxJumpTime;
        }*/
    }

    //预留上升到下降的动画切换脚本,暂时用不上。
/*    void SwitchAnimation() {
        myAnim.SetBool("Idle", false);
        if (myAnim.GetBool("jump")){
            if (rigidBody.velocity.y < 0.0f) {
                myAnim.SetBool("jump", false);
                myAnim.SetBool("fall", true);
            }
        } else if (isGrounded) {
            myAnim.SetBool("fall", false);
            myAnim.SetBool("Idle", true);
        }

    }*/


    void updateVerticalMove()
    {
        // get input and judge what to do
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (jumpPressed)
        {
            if (jumpState == JumpState.Grounded && jumpState == JumpState.InAir)
            {
                // jump pressed on the ground. Omni starts jumping
                jumpState = JumpState.FirstJumped;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpInitVelocity);  // start jumping
            }
            else if (jumpState == JumpState.FirstJumped)
            {
                // jump pressed in the air after the 1st stage jumping, Omni jumps for the 2nd time
                jumpState = JumpState.SecondJumped;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpInitVelocity);  // start jumping
            }
        }
        if(jumpState != JumpState.Grounded && rigidBody.velocity.y == 0)
        {
            // judged as touching ground again, reset jump state
            jumpState = JumpState.Grounded;
        }
    }



    void Shoot()
    {
        // 获取鼠标坐标并转换成世界坐标
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // 因为是2D，用不到z轴。使将z轴的值为0，这样鼠标的坐标就在(x,y)平面上了
        m_mousePosition.z = 0;

        // 子弹角度
        float m_fireAngle = Vector2.Angle(m_mousePosition - this.transform.position, Vector2.up);

        //判断左右
        if (m_mousePosition.x > this.transform.position.x)
        {
            m_fireAngle = -m_fireAngle;
        }
        // 仅液态且还有剩余生命值时可射击，同时若omni无法行动则不能发射
        if (moveableState == MoveableState.Moveable && Input.GetMouseButton(0) && loading <= 0 && currentState == OmniState.Liquid && currentLife > 1)
        {
            //判断鼠标位置是否能发射
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("touch area is UI");
            }
            else
            {
                //生命减少1质量和大小需要被缩小
                DecreaseLifeMassScale();
                // 生成子弹
                GameObject m_bullet = Instantiate(bullet, transform.position, Quaternion.identity) as GameObject;

                // 速度
                m_bullet.GetComponent<Rigidbody2D>().velocity = ((m_mousePosition - transform.position).normalized * bullet.GetComponent<BulletBehavior>().bulletSpeed);

                // 角度
                m_bullet.transform.eulerAngles = new Vector3(0, 0, m_fireAngle);
                loading = 1;
            }

        }
        else
        {
            loading = loading < 0 ? 0 : loading - Time.deltaTime;
        }

    }

    //减少质量和大小生命
    public void DecreaseLifeMassScale()
    {
        currentLife--;
        Vector3 decrese = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 scale = new Vector3(this.transform.localScale.x , this.transform.localScale.y, this.transform.localScale.z);
        //scale -= decrese;
        this.transform.localScale= scale-decrese ;
        //Debug.Log(scale);
        this.rigidBody.mass -=0.2f;

    }
    //增加质量和大小生命
    public void IncreaseLifeMassScale()
    {
        currentLife++;
        Vector3 increse = new Vector3(0.2f, 0.2f, 0.2f);
        Vector3 scale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        this.transform.localScale = scale + increse;
        this.rigidBody.mass += 0.2f;
    }

    void recollectTrigger()
    {
        // 在Omni重新捡回被射击出去的凝胶时的触发函数
        currentLife += 1;
    }

    public void enterCannon(CannonBehaviour cannon)
    {
        // 进入大炮时的行为
        //print("Entering Cannon");
        selfCollider.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;  // 暂时禁用renderer与collider
        rigidBody.gravityScale = 0;
        moveableState = MoveableState.NotMoveable;
        transform.position = cannon.transform.position; // 移动到大炮的位置，并暂时禁用重力与移动、跳跃以与大炮位置同步
    }

    public void leaveCannon()
    {
        // 主动离开大炮（不是被发射出去）时的行为
        selfCollider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;  // 重启renderer与collider
        rigidBody.gravityScale = 1;
        moveableState = MoveableState.Moveable;  // 解除运动限制，重启重力
    }

    public void fireFromCannon(float direction, float speed)
    {
        // 被发射出大炮时的行为
        // 获取鼠标坐标并转换成世界坐标，这部分代码是直接搬的playercontroller里的
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // 因为是2D，用不到z轴。使将z轴的值为0，这样鼠标的坐标就在(x,y)平面上了
        m_mousePosition.z = 0;
        Vector3 cannonPos = this.transform.position;    // 获取自身位置
        Vector2 targetDir = m_mousePosition - cannonPos;    // 计算目标角度
        targetDir.Normalize();  // 对角度vector归一化
        rigidBody.velocity = targetDir * speed; // 赋予速度

        selfCollider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;  // 重启collider与renderer
        rigidBody.gravityScale = 1;
        moveableState = MoveableState.WaitingForMoveable;  // 解除运动限制，重启重力
    }

    void resumeMoveableDetect()
    {
        bool touchAnything = selfCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (touchAnything && moveableState == MoveableState.WaitingForMoveable)
        {
            moveableState = MoveableState.Moveable;   // 若omni在被发射出来后撞到了东西，则恢复可操控状态
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 离开某个触发器时的反应
        if(selfCollider.enabled == false && collision.CompareTag("Barrel"))
        {
            // 当被发射出大炮时，重启collider
            selfCollider.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("bulletDetect") && _BullectDetect_rb.velocity == Vector2.zero)
        {
            //捡起子弹
            //Debug.Log("Player can pick it up");
            /*if (Input.GetKey(KeyCode.F))
            {
                Destroy(collision.transform.parent.gameObject);
                if (currentLife < maxLife)
                {
                    IncreaseLifeMassScale();
                    //PlayerController.IncreaseMassAndScale();
                }
            }*/
        }
        //回血药剂
        if (collision.CompareTag("Healing"))
        {
            //Debug.Log("Player can pick it up");

                Destroy(collision.gameObject);
                if (currentLife < maxLife)
                {
                    //内含加血
                    IncreaseLifeMassScale();
                    //PlayerController.IncreaseMassAndScale();
                }
                //Debug.Log(PlayerController.currentLife);
        }
    }

    
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("bulletDetect") && _BullectDetect_rb.velocity == Vector2.zero)
        {
            Debug.Log("Player CANNOT pick it up");
        }
    }*/
    void beDamaged(int damageAmount)
    {
        // 受伤时被调用的函数，但若HP已经归零则不再造成伤害
        if (currentLife > 0)
        {
            currentLife -= 1;
        }
    }

    void dieDetect()
    {
        // 死亡检测。该函数只检测因HP归零导致的死亡
        if (currentLife <= 0)
        {
            die();
        }
    }

    void dieTrigger()
    {
        // 若角色跌落，则由碰撞触发器触发该函数引起角色死亡
        currentLife = 0;
        die();
    }

    void die()
    {
        // 角色死亡时需要执行的行为
    }

    public enum JumpState
    {
        Grounded,
        InAir,
        FirstJumped,    // the first stage of jumping
        SecondJumped,   // the second stage of jumping
        Landed
    }

    public enum OmniState
    {
        // Omni状态枚举
        Liquid,
        Solid,
        Gas
    }

    public enum MoveableState
    {
        Moveable,
        NotMoveable,
        WaitingForMoveable
    }

    void InvincibleWhenHurt()
    {
        currentLife --;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //posion
        if (collision.gameObject.CompareTag("Poison"))
        {
            totalpoisonHurt++;
            
            Debug.Log(totalpoisonHurt);
            hurt = true;
        }

        if (hurt)
        {
            StartCoroutine("Invincible");
            hurt = false;
        }

    }
    //受伤后的无敌时间
    IEnumerator Invincible()
    {
        yield return new WaitForSeconds(0.1f);
        if (totalpoisonHurt>1)
        {
            DecreaseLifeMassScale();
        }
        else if (totalpoisonHurt==1)
        {
            DecreaseLifeMassScale();
        }
        
        totalpoisonHurt = 0.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
