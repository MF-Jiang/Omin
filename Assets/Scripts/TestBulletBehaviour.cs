using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletBehaviour : MonoBehaviour
{
    float startTime;    // ��¼����ʱ��ʱ��
    public float destroyTime;   // ���巢������Ի�
    public float speed; // �ڵ��ٶ�

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
