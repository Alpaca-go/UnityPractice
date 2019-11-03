using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;

    float jumpTime;

    public bool isCrouch;
    public bool onGround;
    public bool isJump;

    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    public LayerMask ground;

    float xVelocity;

    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;

    Vector2 collStandSize;
    Vector2 collStandOffset;
    Vector2 collCrouchSize;
    Vector2 collCrouchOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        collStandSize = coll.size;
        collStandOffset = coll.offset;
        collCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        collCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);
    }

    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        /*Vector2 pos = transform.position;
        Vector2 offset = new Vector2(-footOffset, 0f);

        RaycastHit2D leftCheck = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, ground);
        Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);*/

        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0), Vector2.down, groundDistance, ground);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0), Vector2.down, groundDistance, ground);


        if (leftCheck || rightCheck) onGround = true;
        else onGround = false;
    }

    void GroundMovement()
    {
        if (crouchHeld) crouch();
        else if (!crouchHeld && isCrouch) standUp();
        else if (!onGround && isCrouch) standUp();

        

        xVelocity = Input.GetAxis("Horizontal");

        if (isCrouch) xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FlipDirection();
    }

    void MidAirMovement()
    {
        if (jumpPressed && onGround && !isJump)
        {
            if (isCrouch && onGround)
            {
                standUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }

            onGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time) isJump = false;
        }
    }

    void FlipDirection()
    {
        if (xVelocity < 0) transform.localScale = new Vector2(-1, 1);
        if (xVelocity > 0) transform.localScale = new Vector2(1, 1);
    }

    void crouch()
    {
        isCrouch = true;
        coll.size = collCrouchSize;
        coll.offset = collCrouchOffset;
    }

    void standUp()
    {
        isCrouch = false;
        coll.size = collStandSize;
        coll.offset = collStandOffset;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, rayDirection*length, color);
        return hit;
    }
}
