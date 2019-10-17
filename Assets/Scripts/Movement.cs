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

    public float speed = 10;
    public float jumpForce = 15;
    public float borderSpeed = 2;

    private bool onGround, onWall;
    private bool isSlide, isClimb;
    //private bool isJump;
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
        freeFall();
        siwtchAnim();
    }

    void FixedUpdate()
    {
        //if (isSlide) wallSlide();
        if (isClimb) wallGrab();
    }

    private void Walk()  //地面移动及转身
    {
        float x = Input.GetAxis("Horizontal");
        float xRaw = Input.GetAxisRaw("Horizontal");

        if (x != 0)
        {
            rb.velocity = new Vector2(x * speed, rb.velocity.y);
            anim.SetFloat("walking", Mathf.Abs(xRaw));
        }
        if (xRaw != 0)
        {
            transform.localScale = new Vector3(xRaw, 1, 1);
        }
    }

    /*private void wallSlide()  //贴墙下滑
    {
        rb.velocity = new Vector2(rb.velocity.x, -borderSpeed);
        anim.SetBool("jumping", false);
        anim.SetBool("falling", false);
        anim.SetBool("grabbing", true);
    }*/

    private void wallGrab()  //爬墙移动
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
        /*if (yRaw != 0)
        {
            transform.localScale = new Vector3(1, yRaw, 1);
        }*/
    }

    private void Jump()  //跳跃
    {
        if (onGround) extraJump = 2;  //落地重置多段跳

        if (Input.GetButtonDown("Jump") && extraJump == 0 && onGround)  //跳完落地
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetBool("jumping", true);
        }

        if (Input.GetButtonDown("Jump") && extraJump > 0)  //空中多段跳
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJump--;
            anim.SetBool("jumping", true);
        }
    }

    private void freeFall()  //优化重力
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;
        }

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;
        }
    }

    private void siwtchAnim()  //动画切换
    {
        if (rb.velocity.y < 0 && !onGround && !onWall)  //平抛
        {
            anim.SetBool("falling", true);
        }

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
        {
            anim.SetBool("falling", false);
        }
    }
}