using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // setting up physic system global parameters
        Physics2D.gravity = new Vector2(0, GlobalPhysicParameter.gravity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
