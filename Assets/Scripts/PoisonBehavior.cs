using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBehavior : MonoBehaviour
{
    public float onScreenTime = 1.5f;

    private void Update()
    {
        Disappear();
    }

    //ʱ������󣬶�Һ��ʧ
    void Disappear()
    {
        if (onScreenTime <= 0.1f)
        {
            Destroy(gameObject);
        }
        else
        {
            onScreenTime -= Time.deltaTime;
        }
    }
}
