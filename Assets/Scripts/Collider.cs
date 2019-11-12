using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public LayerMask ground;
    public BoxCollider2D coll;

    public bool onGround, onWall;
    public bool onLeftWall, onRightWall;
    public int wallDir;

    public Vector2 leftSight, rightSight;
    public Vector2 leftFoot, rightFoot;

    private Color debugCollisionColor = Color.red;

    void Start()
    {
         
    }


    void Update()
    {
        onGround = Physics2D.OverlapCircle(transform.position, 0.2f, ground);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, 0.2f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, 0.2f);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, 0.2f);
    }
}
