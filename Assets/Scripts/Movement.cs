using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    public LayerMask ground;
    //public LayerMask wall;

    public Transform groundCheck, leftCheck, rightCheck;

    private float speed = 10;
    private float x, xRaw;

    private float jumpForce = 15;
    private float jumpMax = 2;
    private float jumpHold = 0.1f;
    private float jumpTime;

    private float borderSpeed = 2;
    
    private bool onGround, onWall;
    private bool isSlide, isClimb;
    private bool isJump;
    //private bool wallGrab;

    private int extraJump;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
        onWall = Physics2D.OverlapCircle(rightCheck.position, 0.2f, ground);

        isSlide = !onGround && onWall;
        isClimb = !onGround && onWall && Input.GetKey(KeyCode.LeftArrow);

        Walk();
        Jump();
        multiJump();
        freeFall();
        siwtchAnim();
    }

    void FixedUpdate()
    {
        //if (isSlide) wallSlide();
        if (isClimb) wallClimb();
    }

    private void Walk()  //地面移动
    {
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
        xRaw = Input.GetAxisRaw("Horizontal");
        if (xRaw != 0) transform.localScale = new Vector3(xRaw, 1, 1);
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

        if (y != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, y * speed);
            anim.SetBool("jumping", false);
            anim.SetBool("falling", false);
            //anim.SetFloat("grabMoving", Mathf.Abs(yRaw));
        }
    }

    /*private void Jump()  //蓄力跳跃
    {
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

    /*private void Jump()  //蓄力跳跃+多段跳跃
    {
        if (onGround) extraJump = 2;
        if (Input.GetButtonDown("Jump")  && !isJump && extraJump > 0)
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
            {
                isJump = false;
                extraJump--;
            }
                
        }
    }*/

    private void Jump()  //普通跳跃
    {
        if (onGround) isJump = false;
        if (Input.GetButtonDown("Jump") && onGround && !isJump)  //地面起跳
        {
            isJump = true;
            rb.velocity = Vector2.up * jumpForce;
            anim.SetBool("jumping", true);
        }

        if (Input.GetButtonDown("Jump") && extraJump > 0)  //空中连跳
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