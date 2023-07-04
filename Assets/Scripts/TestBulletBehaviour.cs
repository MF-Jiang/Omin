using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletBehaviour : MonoBehaviour
{
    float startTime;    // 记录发射时的时间
    public float destroyTime;   // 定义发射后多久自毁
    public float speed; // 炮弹速度

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime >= destroyTime)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(-(transform.right) * speed * Time.deltaTime);
        }
    }
}
