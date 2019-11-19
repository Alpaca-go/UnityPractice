using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CollCheck coll;
    private SwitchAnim anim;

    private float x, xRaw;
    private float y, yRaw;
    public Vector2 dir;

    private float jumpForce = 15;
    
    public bool isMove, isJump;
    public bool isGrab, isClimb, isBlock;

    public int extraJump;
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
        if (isGrab)
        {
            wallClimb(dir);
            anim.basicMove(x, y, Mathf.Abs(rb.velocity.y));
        }
        
        
        dirCheck();
        Jump();
        multiJump();
        freeFall();
        collCheck();
    }

    void FixedUpdate()
    {
        
    }

    private void dirCheck()
    {
        if (x > 0) side = 1;
        if (x < 0) side = -1;
        if (side == -1 && coll.onLeftWall && !coll.onGround) side = 1;
        if (side == 1 && coll.onRightWall && !coll.onGround) side = -1;

        anim.Flip(side);
    }

    private void Walk(Vector2 dir)  //地面移动
    {
        if (isGrab) return;
        float speed = 10;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    private void wallClimb(Vector2 dir)  //爬墙
    {
        if (isBlock) return;
        float climb = 4;
        rb.velocity = new Vector2(rb.velocity.x, dir.y * climb);
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
        if (Input.GetButtonDown("Jump") && ((coll.onGround && !isJump) || extraJump > 0))  //地面起跳 || 空中连跳，都给与相同的上升力
        {
            rb.velocity = Vector2.up * jumpForce;
            anim.SetTrigger("jump");
        }
    }

    private void multiJump()  //多段跳跃
    {
        if (coll.onGround || coll.onWall) extraJump = 2;  //连跳次数
        if (Input.GetButtonDown("Jump")) extraJump--;
    }

    /*private void wallJump()
    {

    }*/

    private void freeFall()  //优化重力
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;

        if (isGrab) rb.gravityScale = 0;
        else rb.gravityScale = 1;
    }

    private void collCheck()
    {
        isJump = (rb.velocity.y == 0 || coll.onWall || coll.onGround) ? false : true;
        isGrab = (coll.onBlock && coll.onWall && !coll.onGround) ? true : false;
        /*bool isLeftGrab = coll.onLeftBlock && coll.onLeftWall && !coll.onGround && Input.GetKeyDown(KeyCode.LeftArrow);
        bool isRightGrab = coll.onRightBlock && coll.onRightWall && !coll.onGround && Input.GetKeyDown(KeyCode.RightArrow);
        isGrab = (isLeftGrab || isRightGrab) ? true : false;*/
        isClimb = (isGrab && rb.velocity.y != 0) ? true : false;
        isBlock = coll.onBlock ? false : true;
    }
}