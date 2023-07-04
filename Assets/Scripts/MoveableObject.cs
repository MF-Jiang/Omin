using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script references the 2D platform game tutorial provided by Unity.
// for further use, this class may need to be re-modified

public class MoveableObject : MonoBehaviour
{
    // members taken from Unity 2D platform game tutorial
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Rigidbody2D rigidBody;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    public float gravity;

    public float minXSpeed = 0.01f;
    public float minYSpeed = 0.01f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // set up default state of this object
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

        // initialize gravity value
        gravity = GlobalPhysicParameter.gravity;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected void moveObject(Vector2 move)
    {

    }
}
