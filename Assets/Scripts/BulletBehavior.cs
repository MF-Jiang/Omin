using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Transform EnterCollisionTranstrom;//发生碰撞时的transfrom
    private Rigidbody2D _rb;
    [SerializeField] private bool isStick = false;
    [SerializeField] private bool canReturn = false;
    public float bulletSpeed = 10f;
    public float RecoverSpeed = 5f;
    public float waitTime = 0.5f;
    [SerializeField] private GameObject player;
    void Awake()
    {
        _rb = this.gameObject.GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (isStick)
        {
            StartCoroutine(StickTimeAfterRecover(waitTime));
        }
        
        //
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnterCollisionTranstrom = gameObject.transform;
        //Debug.Log("yes");
        //碰到tag为wall使子弹停止
        if (collision.gameObject.CompareTag("Wall"))
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
            isStick = true;
        }

        //落出地图，子弹消失
        if (collision.gameObject.name == "Boundary")
        {
            Destroy(gameObject);
        }
    }

    //发射在跷跷板上会跟随跷跷板移动

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Teeterboard")) 
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
            Destroy(GetComponent<Rigidbody2D>());
            gameObject.transform.parent = collision.transform.Find("BULLETSET");
            //gameObject.transform.localScale = new Vector3(0.025f, 0.5f, 0.25f);
            Destroy(GetComponent<CircleCollider2D>());
            
        }
    }


    //回收子弹
    public void RecoverBullet()
    {
        if (Input.GetKeyDown(KeyCode.F)){
            canReturn = true;
        }

        if (canReturn)
        {

            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
            Vector3 playerPositon = player.transform.position;
            this.transform.position = Vector3.MoveTowards(transform.position, playerPositon, RecoverSpeed * Time.deltaTime);
            float distance = (transform.position - playerPositon).magnitude;
            if (distance < 0.3f)
            {
                Destroy(gameObject);

                if (PlayerController.currentLife<PlayerController.maxLife)
                {
                    player.gameObject.GetComponent<PlayerController>().IncreaseLifeMassScale();
                }
               
            }
        }   
    }

    //让bullet能停留一会
    IEnumerator StickTimeAfterRecover(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RecoverBullet();
    }
}
