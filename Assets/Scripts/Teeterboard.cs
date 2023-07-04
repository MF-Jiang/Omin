using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teeterboard : MonoBehaviour
{
    public bool hasForce = false;
    private Rigidbody2D rigidbody;
    private Rigidbody2D originrigidbody;
    private Quaternion originrotation;
    private float Wv;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        originrigidbody = rigidbody;
        originrotation = gameObject.transform.rotation;
        //Wv = Mathf.Sqrt(backForce / (rigidbody.mass * gameObject.transform.lossyScale.x));
    }

    // Update is called once per frame
    void Update()
    {
        if (hasForce == true)
        {
            if (gameObject.transform.rotation.z < -0.01)
            {
                float zAngle = 0.1f;
                zAngle += 0.1f * Time.deltaTime;
                transform.Rotate(0, 0, zAngle, Space.Self);
                //gameObject.transform.rotation = new Quaternion(0, 0, gameObject.transform.rotation.z + Mathf.Epsilon, 0);
            }
            else if (gameObject.transform.rotation.z > 0.01)
            {
                float zAngle = -0.1f;
                zAngle -= 0.1f * Time.deltaTime;
                transform.Rotate(0, 0, zAngle, Space.Self);
                //gameObject.transform.rotation = new Quaternion(0, 0, gameObject.transform.rotation.z - Mathf.Epsilon, 0);
            }
            else if (gameObject.transform.rotation.z > -0.01 && gameObject.transform.rotation.z < 0.01)
            {
                transform.rotation = originrotation;
                rigidbody.angularVelocity = 0;
                rigidbody.rotation = 0;

            }

        }

        /*        if (gameObject.transform.rotation == originrotation && rigidbody == originrigidbody) {

                    hasForce = false;

                }*/
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasForce = false;

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasForce = true;
            rigidbody.angularVelocity = 0;
        }
    }

}
