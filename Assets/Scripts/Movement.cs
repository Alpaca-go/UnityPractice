using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CollisionCheck coll;
    private Animator anim;

    private float speed = 10;
    private float x, xRaw;
    private float y, yRaw;

    private float jumpForce = 15;
    
    private bool isSlide;
    private bool isJump, isBlock;
    //private bool wallGrab;

    private int extraJump;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CollisionCheck>();
        anim = GetComponent<Animator>();

    }


    void Update()
    {
        Walk();
        Jump();
        multiJump();
        freeFall();
        siwtchAnim();

        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //if (isSlide) wallSlide();
        if (coll.onWall) wallClimb();
    }

    private void Walk()  //地面移动
    {
        if (coll.onWall) return;
        x = Input.GetAxis("Horizontal");
        if (x != 0)
        {
            rb.velocity = new Vector2(x * speed, rb.velocity.y);
            anim.SetFloat("walking", Mathf.Abs(xRaw));
        }
        
        if (xRaw != 0) transform.localScale = new Vector3(xRaw, 1, 1);
    }

    private void wallClimb()  //爬墙
    {
        anim.SetBool("jumping", false);
        anim.SetBool("falling", false);
        anim.SetBool("grabbing", true);

        y = Input.GetAxis("Vertical");
        if (y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, y * speed);
            anim.SetFloat("grabMoving", Mathf.Abs(yRaw));
        }
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
        if (coll.onGround || coll.onWall) isJump = false;
        if (Input.GetButtonDown("Jump") && ((coll.onGround && !isJump) || extraJump > 0))  //地面起跳 || 空中连跳，都给与相同的上升力
        {
            isJump = true;
            rb.velocity = Vector2.up * jumpForce;
            anim.SetBool("jumping", true);
        }
    }

    private void multiJump()  //多段跳跃
    {
        if (coll.onGround || coll.onWall) extraJump = 2;  //连跳次数
        if (Input.GetButtonDown("Jump") && extraJump > 0) extraJump--;
    }

    /*private void wallJump()
    {
        if (Input.GetButtonDown("Jump") && )
        {
            onWall = false;
        }
    }*/

    private void freeFall()  //优化重力
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;

        if (coll.onWall) rb.gravityScale = 0;
        else rb.gravityScale = 1;
    }

    private void siwtchAnim()  //动画切换
    {
        if (rb.velocity.y < 0 && !coll.onGround && !coll.onWall)  //平抛
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

        if (coll.onGround)  //落地
        {
            anim.SetBool("falling", false);
            //anim.SetBool("grabbing", false);
        }
    }

    
}