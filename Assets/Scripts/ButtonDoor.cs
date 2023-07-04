using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    private GameObject Door;
    private Rigidbody2D rigid;
    float maxlen = 0;
    float maxposy = 0;

    private void Start()
    {
        Door = gameObject.transform.parent.transform.parent.gameObject.GetComponentInChildren<Door>().gameObject;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        maxlen = Door.transform.localScale.y;
        maxposy = rigid.position.y;
    }
    private void Update()
    {
        if (rigid.velocity.y > 0.1f)
        {
            if (Door.transform.localScale.y < maxlen)
            {
                Door.transform.localScale = new Vector2(Door.transform.localScale.x, Door.transform.localScale.y + 0.5f);
            }
        }
        else if (rigid.velocity.y < -0.1f)
        {
            
            if (Door.transform.localScale.y > 0.0f)
            {
                Door.transform.localScale = new Vector2(Door.transform.localScale.x, Door.transform.localScale.y - 1.5f);
            }
        }

    }

}
