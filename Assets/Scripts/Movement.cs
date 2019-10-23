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

    public Transform groundCheck, rightCheck;

    public float speed_x = 0.1f;
    public float speed_y = 0.1f;
    public float gravity = 0.1f;
    public float grabbing_speed = 0.01f;
    //public float jumpForce = 350;

    private bool isGround;
    private bool onWall;
    private bool isJump;

    public int allowJumpTimes = 2;

    private float v_x = 0f;
    private float v_y = 0f;
    private int jumpTimes;

    private Transform trans;

    void Start()
    {
        Debug.Log("start");
        trans = GetComponent<Transform>();

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        jumpTimes = allowJumpTimes;
    }


    void Update()
    {
        UpdateStatus();
        
    }

    void FixedUpdate()
    {
        UpdateAnim();
        UpdatePosition();
        
    }

    private void UpdateStatus()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
        onWall = Physics2D.OverlapCircle(rightCheck.position, 0.2f, ground);

        if (!isGround && !onWall)
        {
            v_y -= gravity;
        }
        if (isGround)
        {
            isJump = false;
            v_y = 0;
            jumpTimes = allowJumpTimes; 
        }

        if (onWall && y != 0f)
        {
            v_x = 0;
            v_y = grabbing_speed * y;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (jumpTimes > 0)
            {
                onWall = false;
                isJump = true;
                v_y += speed_y;
                jumpTimes--;
                if (jumpTimes < 0)
                    jumpTimes = 0;
            }
        }

        v_x = x * speed_x;
    }

    private void UpdatePosition()
    {
        if (v_x != 0f)
        {
            transform.localScale = new Vector3(v_x > 0f?1:-1, 1, 1);
        }
        trans.localPosition = new Vector3(trans.localPosition.x + v_x * Time.fixedDeltaTime, trans.localPosition.y + v_y * Time.fixedDeltaTime, trans.localPosition.z);
    }

    private void UpdateAnim()
    {
        if (isJump)
        {
            anim.SetBool("jumping", true);
        } else
        {
            anim.SetBool("jumping", false);
        }
        if (onWall)
        {
            anim.SetBool("grabbing", true);
        }else
        {
            anim.SetBool("grabbing", false);
        }
        if (v_y != 0f)
        {
            anim.SetInteger("onVerticalMove", v_y > 0f ? 1 : -1);
        }
        else
        {
            anim.SetInteger("onVerticalMove", 0);
        }

        if (v_x != 0f)
        {
            anim.SetInteger("onHorizontalMove", v_x > 0f ? 1 : -1);
        }else
        {
            anim.SetInteger("onHorizontalMove", 0);
        }
    }

    //private void Walk()
    //{
    //    float x = Input.GetAxis("Horizontal");
    //    float xRaw = Input.GetAxisRaw("Horizontal");

    //    if (x != 0)
    //    {
    //        //Debug.Log(rb.velocity);
    //        rb.velocity = new Vector2(x * speed, rb.velocity.y);
    //        anim.SetFloat("walking", Mathf.Abs(xRaw));
    //    }
    //    if (xRaw != 0)
    //    {
    //        transform.localScale = new Vector3(xRaw, 1, 1);
    //    }
    //}

    //private void Grab()
    //{
    //    float y = Input.GetAxis("Vertical");
    //    //float yRaw = Input.GetAxisRaw("Vertical");

    //    if (rb.velocity.y != 0 && Input.GetKey(KeyCode.LeftArrow) && onRightWall)
    //    {
    //        //transform.localScale = new Vector3(1, 1, 1);
    //        rb.velocity = new Vector2(0, y * speed);
    //        anim.SetBool("jumping", false);
    //        anim.SetBool("falling", false);
    //        anim.SetBool("grabbing", true);
    //    }
    //}

    //private void ActJump() 
    //{
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        if (jumpTimes > 0) {
    //            // rb.velocity = Vector2.up * jumpForce;
    //            rb.AddForce(Vector2.up * jumpForce);
                
    //            anim.SetBool("jumping", true);
    //            jumpTimes --;
    //            if (jumpTimes < 0) jumpTimes = 0;
    //            isJump = true;
    //        }
    //    }
    //}

    //private void Jump()
    //{
    //    if (isJump) {
    //        if (isGround && rb.velocity.y < 0) {
    //            anim.SetBool("jumping", false);
    //            isJump = false;
    //            jumpTimes = allowJumpTimes;
    //        }
    //    }
    //}

    //private void switchAnim()
    //{
    //    if (rb.velocity.y < 0 && !coll.IsTouchingLayers(ground))
    //    {
    //        anim.SetBool("falling", true);
    //    }

    //    if (anim.GetBool("jumping"))
    //    {
    //        if (rb.velocity.y < 0)
    //        {
    //            anim.SetBool("jumping", false);
    //            anim.SetBool("falling", true);
    //        }

    //        if(rb.velocity.y > 0)
    //        {
    //            anim.SetBool("jumping", true);
    //            anim.SetBool("falling", false);
    //        }
    //    }
        

    //    if (coll.IsTouchingLayers(ground))
    //    {
    //        anim.SetBool("falling", false);
    //    }
    //}

}
