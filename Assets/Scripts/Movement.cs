using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    public LayerMask ground;
    //public LayerMask wall;

    //public Transform groundCheck, leftCheck, rightCheck;

    private float speed = 10;
    private float x, xRaw;

    private float jumpForce = 15;
    

    private bool onGround, onWall;
    private bool isSlide;
    private bool isJump, isBlock;
    //private bool wallGrab;

    private int extraJump;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

    }


    void Update()
    {
        rayCheck();
        Walk();
        Jump();
        multiJump();
        freeFall();
        siwtchAnim();
    }

    void FixedUpdate()
    {
        //if (isSlide) wallSlide();
        if (onWall) wallClimb();
    }

    RaycastHit2D Raycast(Vector2 offSet, Vector2 rayDir, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offSet, rayDir, length, layer);
        Debug.DrawRay(pos + offSet, rayDir * length);
        return hit;
    }

    private void rayCheck()
    {
        Vector2 coo = coll.offset;
        Vector2 cos = coll.size;
        
        float dir = transform.localScale.x;
        Vector2 climbDir = new Vector2(dir, 0);

        RaycastHit2D leftFoot = Raycast(new Vector2((-cos.x / 2 + coo.x) * dir, -cos.y), Vector2.down, 0.2f, ground);
        RaycastHit2D rightFoot = Raycast(new Vector2((cos.x / 2 + coo.x) * dir, -cos.y), Vector2.down, 0.2f, ground);
        if (leftFoot || rightFoot) onGround = true;
        else onGround = false;

        RaycastHit2D leftHead = Raycast(new Vector2((-cos.x / 2 + coo.x) * dir, 0), Vector2.up, 0.2f, ground);
        RaycastHit2D rightHead = Raycast(new Vector2((cos.x / 2 + coo.x) * dir, 0), Vector2.up, 0.2f, ground);
        if (leftHead || rightHead) isBlock = true;
        else isBlock = false;

        RaycastHit2D leftSight = Raycast(new Vector2((-cos.x / 2 + coo.x) * dir, 0), -climbDir, 0.2f, ground);
        RaycastHit2D rightSight = Raycast(new Vector2((cos.x / 2 + coo.x) * dir, 0), climbDir, 0.2f, ground);
        if ((leftSight && !onGround) || (rightSight && !onGround)) onWall = true;
        else onWall = false;
    }

    private void Walk()  //地面移动
    {
        if (onWall) return;
        x = Input.GetAxis("Horizontal");
        if (x != 0)
        {
            rb.velocity = new Vector2(x * speed, rb.velocity.y);
            anim.SetFloat("walking", Mathf.Abs(xRaw));
        }
        Flip();
    }

    private void Flip()  //转身
    {
        int side = (onWall) ? -1 : 1;
        xRaw = Input.GetAxisRaw("Horizontal");
        if (xRaw != 0) transform.localScale = new Vector3(xRaw * side, 1, 1);
    }

    /*private void wallSlide()  //贴墙下滑
    {
        rb.velocity = new Vector2(rb.velocity.x, -borderSpeed);
        anim.SetBool("jumping", false);
        anim.SetBool("falling", false);
        anim.SetBool("grabbing", true);
    }*/

    private void wallClimb()  //爬墙移动
    {
        float y = Input.GetAxis("Vertical");
        //float yRaw = Input.GetAxisRaw("Vertical");

        anim.SetBool("jumping", false);
        anim.SetBool("falling", false);
        anim.SetBool("grabbing", true);

        if (y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, y * speed);
            
            //anim.SetFloat("grabMoving", Mathf.Abs(yRaw));
        }
        Flip();
    }

    /*private void Jump()  //蓄力跳跃
    {
        float jumpMax = 2;
        float jumpHold = 0.1f;
        float jumpTime;

        if (Input.GetButtonDown("Jump") && !isJump && extraJump > 0)
        {
            isJump = true;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpTime = Time.time + jumpHold;
            anim.SetBool("jumping", true);
        }
        else if (isJump)
        {
            if (Input.GetButton("Jump"))
                rb.AddForce(new Vector2(0, jumpMax), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
    }*/

    private void Jump()  //普通跳跃
    {
        if (onGround) isJump = false;
        if (Input.GetButtonDown("Jump") && ((onGround && !isJump) || extraJump > 0))  //地面起跳 || 空中连跳，都给与相同的上升力
        {
            isJump = true;
            rb.velocity = Vector2.up * jumpForce;
            anim.SetBool("jumping", true);
        }
    }

    private void multiJump()  //多段跳跃
    {
        if (onGround) extraJump = 2;  //连跳次数
        if (Input.GetButtonDown("Jump") && extraJump > 0) extraJump--;
    }

    private void freeFall()  //优化重力
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;

        if (onWall) rb.gravityScale = 0;
        else rb.gravityScale = 1;
    }

    private void siwtchAnim()  //动画切换
    {
        if (rb.velocity.y < 0 && !onGround && !onWall)  //平抛
            anim.SetBool("falling", true);

        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)  //下坠
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }

            if (rb.velocity.y > 0)  //起跳
            {
                anim.SetBool("jumping", true);
                anim.SetBool("falling", false);
            }
        }

        if (onGround)  //落地
            anim.SetBool("falling", false);
    }

    
}