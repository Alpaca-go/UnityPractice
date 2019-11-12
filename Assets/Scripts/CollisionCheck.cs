using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public LayerMask ground;

    public bool onGround, onWall;
    public bool onLeftWall, onRightWall;

    public int wallDir;

    public Vector2 leftSight, rightSight;
    public Vector2 leftFoot, rightFoot;

    private Color debugCollisionColor = Color.red;

    void Start()
    {
        leftSight = new Vector2(-0.45f, -0.1f);
        rightSight = new Vector2(0.45f, -0.1f);
        leftFoot = new Vector2(-0.35f, -1);
        rightFoot = new Vector2(0.35f, -1);
    }


    void Update()
    {
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftSight, 0.1f, ground);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightSight, 0.1f, ground);
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + leftFoot, 0.1f, ground) || Physics2D.OverlapCircle((Vector2)transform.position + rightFoot, 0.1f, ground);
        onWall = onLeftWall || onRightWall;

        wallDir = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { leftSight, rightSight, leftFoot, rightFoot };

        Gizmos.DrawWireSphere((Vector2)transform.position + leftSight, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightSight, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftFoot, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightFoot, 0.1f);
    }
}
