using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    BoxCollider2D omniDetector; // ��omniDetector������
    Transform barrelTransform;  // ���ڹܵ�transform�������
    BoxCollider2D barrelCollider;   // ���ڹܵ�collider����
    BoxCollider2D baseCollider; // �Ե���collider������
    public CannonState state = CannonState.Enemy;  // ����Ĭ�ϳ�ʼΪ�ж�״̬

    public GameObject bullet;  // ��bullet������
    public float bulletSpeed; // bullet�ٶ�
    public float fireTime;  // ������÷���һ���ڵ�����λΪ��
    float timeAtLastPeroid; // fixed update����һ�����ڿ�ʼ��ʱ��

    public float omniFireSpeed; // omni������ʱ�ĳ��ٶ�

    PlayerController playerController;  // ����ҿ�����������

    // Start is called before the first frame update
    void Start()
    {
        barrelTransform = transform.Find("Barrel");
        barrelCollider = barrelTransform.GetComponent<BoxCollider2D>();
        baseCollider = transform.Find("Base").GetComponent<BoxCollider2D>();
        timeAtLastPeroid = Time.time;   // ��ʼ����ʱ��
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
        // ��omni��ײ��������������ʾ������ڵ�UI
        if (collision.CompareTag("Player") && state == CannonState.Ally)
        {
            if (Input.GetKey(KeyCode.E))
            {
                // ��E�������
                playerController = collision.GetComponent<PlayerController>();
                playerController.enterCannon(this);
                state = CannonState.Ally_Firing;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �ú���������omni�ɳ�cannon֮������collider�Ĵ���
        if (collision.CompareTag("Player") && state == CannonState.Ally && barrelCollider.enabled == false)
        {
            barrelCollider.enabled = true;
            baseCollider.enabled = true;    // �������ڵ�collider
        }
    }

    // ������Ϊ�з���λʱ����Ϊrunnable����
    void enemyStateBehaviourRunnable()
    {
        Quaternion direction = barrelTransform.rotation;   // �ڵ����䷽��
        if(Time.time - timeAtLastPeroid > fireTime)
        {
            // ��ʱ�����ڵ�
            fireBullet();
            timeAtLastPeroid = Time.time;
        }
    }

    void allyStateBehaviourRunnable()
    {
        if (state == CannonState.Ally_Firing)
        {
            // ����׼
            playerAim();
            // �������
            if (Input.GetKey(KeyCode.Space))
            {
                // �����omni�������playerController������
                baseCollider.enabled = false;
                barrelCollider.enabled = false; // ����barrel��base����ײ���Ա������omni
                playerController.transform.position = new Vector3(barrelTransform.position.x, barrelTransform.position.y, playerController.transform.position.z);
                playerController.fireFromCannon(barrelTransform.rotation.z, omniFireSpeed); // ��omni�Ƶ�����λ���ϲ������ȥ
                playerController = null;
                // �ص�ally״̬
                state = CannonState.Ally;
            }
        }
    }

    void playerAim()
    {
        // ��Ҳ���������׼�ĺ���
        // ��ȡ������겢ת�����������꣬�ⲿ�ִ�����ֱ�Ӱ��playercontroller���
        Vector3 m_mousePosition = Input.mousePosition;
        m_mousePosition = Camera.main.ScreenToWorldPoint(m_mousePosition);
        // ��Ϊ��2D���ò���z�ᡣʹ��z���ֵΪ0�����������������(x,y)ƽ������
        m_mousePosition.z = 0;
        Vector3 cannonPos = this.transform.position;    // ��ȡ����λ��
        Vector2 targetDir = m_mousePosition - cannonPos;    // ����Ŀ��Ƕ�
        float targetAngle = Vector2.Angle(targetDir, Vector3.up);   // �������λ�������λ�ýǶ�
        if(m_mousePosition.x > cannonPos.x)
        {
            targetAngle = -targetAngle; // ������ڴ����ұ�����Ҫ�ԽǶȷ���
        }
        barrelTransform.transform.eulerAngles = new Vector3(0, 0, targetAngle - 90);    // ��תʹ���ڶ�׼��Ӧ�Ƕ�
    }

    void stateChange()
    {
        state = CannonState.Ally;   // �л�������̬�������л�ʱ��Ҫִ�еĶ���ҲӦ���ڸú�����
    }

    void fireBullet()
    {
        GameObject newBullet = Instantiate(bullet) as GameObject;
        newBullet.transform.position = barrelTransform.position;
        newBullet.transform.rotation = barrelTransform.rotation;   // �趨�ڵ�����Ƕ�
        newBullet.GetComponent<TestBulletBehaviour>().speed = bulletSpeed;   // �趨�ڵ��ٶ�
    }

    public enum CannonState
    {
        Enemy,
        Ally,
        Ally_Firing,
    }
}
