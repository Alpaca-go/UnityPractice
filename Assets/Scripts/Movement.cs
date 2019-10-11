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
    public float jumpForce = 6;

    private bool isGround;
    private bool onLeftWall;
    private bool onRightWall;
    private bool isJump;

    private int extraJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        //transform.DetachChildren();
    }

    
    void Update()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
        onLeftWall = Physics2D.OverlapCircle(leftCheck.position, 0.2f, ground);
        onRightWall = Physics2D.OverlapCircle(rightCheck.position, 0.2f, ground);

        Walk();
        Jump();
        Grab();
        siwtchAnim();


    }

    private void Walk()
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

    private void Grab()
    {
        float y = Input.GetAxis("Vertical");
        //float yRaw = Input.GetAxisRaw("Vertical");

        if (rb.velocity.y != 0 && Input.GetKey(KeyCode.LeftArrow) && onRightWall)
        {
            //transform.localScale = new Vector3(1, 1, 1);
            rb.velocity = new Vector2(0, y * speed);
            anim.SetBool("jumping", false);
            anim.SetBool("falling", false);
            anim.SetBool("grabbing", true);
        }
    }





    private void Jump()
    {
        float fallDown = 3f;
        float upResis = 2f;

        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }

        if (isGround)
        {
            extraJump = 2;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;
        }

        if (rb.velocity.y > 0 && !isJump)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;
        }

        if (isJump && extraJump > 0)
        {
            //rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = Vector2.up * jumpForce;
            extraJump--;
            anim.SetBool("jumping", true);
        }
        if (isJump && extraJump == 0 && isGround)
        {
            //rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = Vector2.up * jumpForce;
            anim.SetBool("jumping", true);
        }
    }

    private void siwtchAnim()
    {
        if (rb.velocity.y < 0 && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", true);
        }

        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }

            if(rb.velocity.y > 0)
            {
                anim.SetBool("jumping", true);
                anim.SetBool("falling", false);
            }
        }
        

        if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
        }
    }

}
