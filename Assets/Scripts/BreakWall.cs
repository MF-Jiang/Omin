using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    private int life = 2;
    private GameObject[] Bullets = {null,null};
    private GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PlayerController.currentLife);
        if (life <= 0) {
            if (Bullets[0] != null && Bullets[1] != null)
            {
                Player.GetComponent<PlayerController>().IncreaseLifeMassScale();
                Player.GetComponent<PlayerController>().IncreaseLifeMassScale();
                //PlayerController.currentLife += 2;
                Destroy(Bullets[0]);
                Destroy(Bullets[1]);
            }
            else if (Bullets[0] != null && Bullets[1] == null) {
                Player.GetComponent<PlayerController>().IncreaseLifeMassScale();
                //PlayerController.currentLife += 1;
                Destroy(Bullets[0]);
            }

            StartCoroutine(WaitDestroy());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) {
            if (Bullets[0] == null)
            {
                Bullets[0] = collision.gameObject;
            }
            else if (Bullets[1] == null)
            {
                Bullets[1] = collision.gameObject;
            }
            //Player.GetComponent<PlayerController>().DecreaseLifeMassScale();
            life -= 1;
        }
    }

    IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
