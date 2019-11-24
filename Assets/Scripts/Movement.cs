using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private CollCheck coll;
    private Rigidbody2D rb;
    private SwitchAnim anim;

    private float x, xRaw;
    private float y, yRaw;
    public Vector2 dir;

    private float speed = 10;
    private float jumpForce = 15;
    private float wallJumpLerp = 10;
    private float dashSpeed = 20;


    public bool canMove, isGrab, isClimb;
    public bool wallJumped, wallPushed;
    public bool isDashing, hasDashed;

    public bool groundTouch;

    public int side = 1;
    public int extraJump;

    void Start()
    {
        coll = GetComponent<CollCheck>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<SwitchAnim>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        dir = new Vector2(x, y);

        DirCheck();


        Walk(dir);
        anim.basicMove(Mathf.Abs(x), y, rb.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if (coll.onGround) Jump(Vector2.up, false);
            if (coll.onWall) WallJump();
        }

    }

    private void DirCheck()
    {
        if (x > 0) side = 1;
        if (x < 0) side = -1;

        anim.Flip(side);
    }

    private void Walk(Vector2 dir)  //基本移动
    {
        if (!canMove) return;
        if (isGrab) return;

        if (!wallJumped)
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        else
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(dir.x * speed, rb.velocity.y), wallJumpLerp * Time.deltaTime);
    }

    private void Jump(Vector2 dir, bool wall)  //基本跳跃
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    /*private void MultiJump()
    {
        if (coll.onGround || coll.onWall) extraJump = 2;
    }*/

    private void WallJump()  //蹬墙跳跃
    {
        if ((side == 1 && coll.onRightWall) || (side == -1 && coll.onLeftWall))
            anim.Flip(side * -1);

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        Jump(Vector2.up + wallDir, true);
        wallJumped = true;
    }

    private void WallGrab()  //抓墙静止
    {
        if (coll.wallDir != side) anim.Flip(side * -1);
        if (!canMove) return;

        //取消抓墙时的反冲力
        /*bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
            pushingWall = true;
        float push = pushingWall ? 0 : rb.velocity.x;
        rb.velocity = new Vector2(push, 0);*/
    }

    private void WallClimb()
    {
        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(dir.x * speed, rb.velocity.y), wallJumpLerp * Time.deltaTime);
        }
    }
}
