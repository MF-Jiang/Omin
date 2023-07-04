using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MoveableObject
{
    // ��ɫ�˶�����
    [Header ("=========Character motion parameters=========")]
    public float moveSpeed = 4;
    public float maxJumpHeight = 2;
    public int maxJumpTime = 2;
    float jumpInitVelocity;
    public int curJumpCount;
    public bool isGrounded = true;
    public JumpState jumpState = JumpState.Grounded;

    public bool couldDoubleJump = true;

    public float curSpriteSize; // ��ɫspriteĿǰ�Ĵ�С��defaultΪ1
    [SerializeField]private Rigidbody2D _BullectDetect_rb;

    // ��ɫ״̬�����
    public OmniState initState = OmniState.Liquid;
    public OmniState currentState;
    public MoveableState moveableState = MoveableState.Moveable;

    // ��ɫ����ֵ�����
    public static float maxLife = 5.0f;
    public static float currentLife;
    // �յ����˺�;
    public float totalpoisonHurt = 0.0f;
    public bool hurt =false;
    // ��sprite�������������
    BoxCollider2D selfCollider;

    [Header ("====== Jump variable=========")]
    
    float hMove;
    bool jumpPressed;

    [Header("====== Shoot variable========")]
    //�ӵ���gameobject
    public GameObject bullet;
    private float loading = 0;
    public bool shootAction = false;

    //��ɫ����������
    private Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<BoxCollider2D>();   // ��ʼ��rigidbody��collider����

        // ��ʼ���˶�����
        jumpInitVelocity = Mathf.Sqrt( 2 * maxJumpHeight * Mathf.Abs(GlobalPhysicParameter.gravity) ); // �����趨�������������ٶ�
        curJumpCount = maxJumpTime; // ��ʼ����Ծ����
        hMove = 0;
        jumpPressed = false;

        // ��ʼ������ֵ��״̬
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
            updateJumping();    // ֻ���������˶�ʱ������Ծ���ƶ�
        }
        else if (moveableState == MoveableState.WaitingForMoveable)
        {
            resumeMoveableDetect();
        }
        
        Shoot();
        dieDetect();

        
        //SwitchAnimation() Ԥ��һ�����������䶯���л��ĺ���
        //��ɫ����
        if (currentLife <= Mathf.Epsilon) {
            Destroy(gameObject);
        }
    }

    //����ǲ����ڵ�����
    void CheckGrounded() {
        isGrounded = selfCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isGrounded && rigidBody.velocity.y == 0) {
            curJumpCount = maxJumpTime;
        }
        //Debug.Log(isGrounded);
    }

    //������и��������Ĳ���������fixedupdate�У���ֹ����
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
        // �ú������sprite�Ƿ�Ӵ��˵���/��������ŵ�����

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

    //Ԥ���������½��Ķ����л��ű�,��ʱ�ò��ϡ�
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
        // ��ȡ������겢ת������������
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // ��Ϊ��2D���ò���z�ᡣʹ��z���ֵΪ0�����������������(x,y)ƽ������
        m_mousePosition.z = 0;

        // �ӵ��Ƕ�
        float m_fireAngle = Vector2.Angle(m_mousePosition - this.transform.position, Vector2.up);

        //�ж�����
        if (m_mousePosition.x > this.transform.position.x)
        {
            m_fireAngle = -m_fireAngle;
        }
        // ��Һ̬�һ���ʣ������ֵʱ�������ͬʱ��omni�޷��ж����ܷ���
        if (moveableState == MoveableState.Moveable && Input.GetMouseButton(0) && loading <= 0 && currentState == OmniState.Liquid && currentLife > 1)
        {
            //�ж����λ���Ƿ��ܷ���
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("touch area is UI");
            }
            else
            {
                //��������1�����ʹ�С��Ҫ����С
                DecreaseLifeMassScale();
                // �����ӵ�
                GameObject m_bullet = Instantiate(bullet, transform.position, Quaternion.identity) as GameObject;

                // �ٶ�
                m_bullet.GetComponent<Rigidbody2D>().velocity = ((m_mousePosition - transform.position).normalized * bullet.GetComponent<BulletBehavior>().bulletSpeed);

                // �Ƕ�
                m_bullet.transform.eulerAngles = new Vector3(0, 0, m_fireAngle);
                loading = 1;
            }

        }
        else
        {
            loading = loading < 0 ? 0 : loading - Time.deltaTime;
        }

    }

    //���������ʹ�С����
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
    //���������ʹ�С����
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
        // ��Omni���¼�ر������ȥ������ʱ�Ĵ�������
        currentLife += 1;
    }

    public void enterCannon(CannonBehaviour cannon)
    {
        // �������ʱ����Ϊ
        //print("Entering Cannon");
        selfCollider.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;  // ��ʱ����renderer��collider
        rigidBody.gravityScale = 0;
        moveableState = MoveableState.NotMoveable;
        transform.position = cannon.transform.position; // �ƶ������ڵ�λ�ã�����ʱ�����������ƶ�����Ծ�������λ��ͬ��
    }

    public void leaveCannon()
    {
        // �����뿪���ڣ����Ǳ������ȥ��ʱ����Ϊ
        selfCollider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;  // ����renderer��collider
        rigidBody.gravityScale = 1;
        moveableState = MoveableState.Moveable;  // ����˶����ƣ���������
    }

    public void fireFromCannon(float direction, float speed)
    {
        // �����������ʱ����Ϊ
        // ��ȡ������겢ת�����������꣬�ⲿ�ִ�����ֱ�Ӱ��playercontroller���
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // ��Ϊ��2D���ò���z�ᡣʹ��z���ֵΪ0�����������������(x,y)ƽ������
        m_mousePosition.z = 0;
        Vector3 cannonPos = this.transform.position;    // ��ȡ����λ��
        Vector2 targetDir = m_mousePosition - cannonPos;    // ����Ŀ��Ƕ�
        targetDir.Normalize();  // �ԽǶ�vector��һ��
        rigidBody.velocity = targetDir * speed; // �����ٶ�

        selfCollider.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;  // ����collider��renderer
        rigidBody.gravityScale = 1;
        moveableState = MoveableState.WaitingForMoveable;  // ����˶����ƣ���������
    }

    void resumeMoveableDetect()
    {
        bool touchAnything = selfCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (touchAnything && moveableState == MoveableState.WaitingForMoveable)
        {
            moveableState = MoveableState.Moveable;   // ��omni�ڱ����������ײ���˶�������ָ��ɲٿ�״̬
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �뿪ĳ��������ʱ�ķ�Ӧ
        if(selfCollider.enabled == false && collision.CompareTag("Barrel"))
        {
            // �������������ʱ������collider
            selfCollider.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("bulletDetect") && _BullectDetect_rb.velocity == Vector2.zero)
        {
            //�����ӵ�
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
        //��Ѫҩ��
        if (collision.CompareTag("Healing"))
        {
            //Debug.Log("Player can pick it up");

                Destroy(collision.gameObject);
                if (currentLife < maxLife)
                {
                    //�ں���Ѫ
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
        // ����ʱ�����õĺ���������HP�Ѿ�������������˺�
        if (currentLife > 0)
        {
            currentLife -= 1;
        }
    }

    void dieDetect()
    {
        // ������⡣�ú���ֻ�����HP���㵼�µ�����
        if (currentLife <= 0)
        {
            die();
        }
    }

    void dieTrigger()
    {
        // ����ɫ���䣬������ײ�����������ú��������ɫ����
        currentLife = 0;
        die();
    }

    void die()
    {
        // ��ɫ����ʱ��Ҫִ�е���Ϊ
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
        // Omni״̬ö��
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
    //���˺���޵�ʱ��
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
