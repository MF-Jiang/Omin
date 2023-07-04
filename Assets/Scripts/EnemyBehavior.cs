using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBehavior : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int enemyLives = 2;
    [Header("time")]
    public float startTime;
    public float waitTime;
    public Transform leftPosition;
    public Transform rightPosition;
    public Transform movePosition;

    public GameObject poison;
    public GameObject poisonArea;
    private BoxCollider2D collider1;
    private Rigidbody2D _rb;

    private bool isHurt = false;

    //private bool isRight = true;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        collider1 = GetComponent<BoxCollider2D>();
        waitTime = startTime;
        movePosition.position = leftPosition.position;
        InvokeRepeating("GeneratePoison", 0.5f, 0.15f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHurt) {
            EnemyMoving();
        }
        
        EnemyDied();

    }

    void EnemyMoving()
    {
        //没有使用刚体的移动函数，veloctiy为（0，0）
        transform.position = Vector2.MoveTowards
                            (transform.position, movePosition.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, leftPosition.position) < 0.1f)
            if (waitTime <= 0)
            {
                waitTime = startTime;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                movePosition.position = rightPosition.position;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }

        if (Vector2.Distance(transform.position, rightPosition.position) < 0.1f)
        {
            if (waitTime <= 0)
            {
                waitTime = startTime;
                //x转向
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                //transform.localScale,如果有父物体子物体，该方法会导致图片变形
                movePosition.position = leftPosition.position;
                //isRight = false;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
    
    void EnemyDied()
    {
        if (enemyLives <= 0)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (transform.position.x < player.transform.position.x)
            {
                isHurt = true;
                _rb.velocity = new Vector2(-5, _rb.velocity.y);
                StartCoroutine(endHurt());
            }
            else if (transform.position.x > player.transform.position.x) {
                isHurt = true;
                _rb.velocity = new Vector2(5, _rb.velocity.y);
                StartCoroutine(endHurt());
            }
            Destroy(collision.gameObject); //摧毁子弹
            Debug.Log("HIT!!!");
            enemyLives -= 1;
        }
        //遇到玩家兴奋起来了
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Speedup!!");
            movePosition.position = collision.gameObject.transform.position;
        }
        else
        {

        }
    }
    //留下毒液
    void GeneratePoison()
    {
        GameObject m_poison = Instantiate(poison, this.gameObject.transform.position - new Vector3(0,collider1.size.y/2*(gameObject.transform.localScale.y),0), Quaternion.identity) as GameObject;
        m_poison.transform.parent = poisonArea.transform;
    }

    IEnumerator endHurt() {
        yield return new WaitForSeconds(1.0f);
        isHurt = false;
    }
}
