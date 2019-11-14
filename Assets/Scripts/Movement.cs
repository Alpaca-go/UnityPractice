using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CollCheck coll;
    private SwitchAnim anim;

    private float speed = 10;
    private float x, xRaw;
    private float y, yRaw;
    public Vector2 dir;

    private float jumpForce = 15;
    
    public bool isSlide, isClimb;
    public bool isJump, isBlock;
    //private bool wallGrab;

    private int extraJump;
    public int side = 1;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CollCheck>();
        anim = GetComponent<SwitchAnim>();

    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");

        dir = new Vector2(x, y);


        
        Walk(dir);
        anim.basicMove(x, y, rb.velocity.y);

        dirCheck();
        Jump();
        multiJump();
        freeFall();
    }

    void FixedUpdate()
    {
        //if (isSlide) wallSlide();
        //if (coll.onGround) groundTouch();
        if (coll.onWall) wallClimb();
    }

    private void dirCheck()
    {
        if (x > 0) side = 1;
        if (x < 0) side = -1;
        anim.Flip(side);
    }

    private void groundTouch()
    {
        side = anim.sr.flipX ? -1 : 1;
    }
    private void Walk(Vector2 dir)  //地面移动
    {
        if (coll.onWall) return;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y); 
    }

    private void wallClimb()  //爬墙
    {
        isClimb = true;
        y = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, y * speed);
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
}