using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerController player;
    public Image healthMeter;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            //Debug.Log(PlayerController.currentLife);
            healthMeter.fillAmount = PlayerController.currentLife/ PlayerController.maxLife;

        }
        else
        {
            healthMeter.fillAmount = 0;
        }
    }
}
