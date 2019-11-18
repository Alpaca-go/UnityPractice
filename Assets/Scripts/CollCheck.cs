using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollCheck : MonoBehaviour
{
    public LayerMask ground;

    public bool onLeftBlock, onRightBlock;
    public bool onLeftWall, onRightWall;
    public bool onGround, onWall, onBlock;

    public int wallDir;

    private Vector2 leftSight, rightSight;
    private Vector2 leftBody, rightBody;
    private Vector2 leftFoot, rightFoot;

    void Start()
    {
        leftSight = new Vector2(-0.45f, 0.45f);
        rightSight = new Vector2(0.45f, 0.45f);
        leftBody = new Vector2(-0.45f, -0.1f);
        rightBody = new Vector2(0.45f, -0.1f);
        leftFoot = new Vector2(-0.35f, -1);
        rightFoot = new Vector2(0.35f, -1);
    }


    void Update()
    {
        onBlock = Physics2D.OverlapCircle((Vector2)transform.position + leftSight, 0.1f, ground) || Physics2D.OverlapCircle((Vector2)transform.position + rightSight, 0.1f, ground);

        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftBody, 0.1f, ground);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightBody, 0.1f, ground);
        onWall = onLeftWall || onRightWall;

        onGround = Physics2D.OverlapCircle((Vector2)transform.position + leftFoot, 0.1f, ground) || Physics2D.OverlapCircle((Vector2)transform.position + rightFoot, 0.1f, ground);

        wallDir = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + leftSight, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightSight, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftBody, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightBody, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftFoot, 0.1f);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightFoot, 0.1f);
    }
}
