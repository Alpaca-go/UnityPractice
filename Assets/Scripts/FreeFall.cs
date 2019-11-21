using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFall : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;
    }

    public void freeFall()  //优化重力
    {
        float fallDown = 2f;  //重力修正
        float upResis = 2f;  //上升阻力

        if (rb.velocity.y < 0)  //自由落体运动
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallDown * Time.deltaTime;

        if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump"))   //添加上升阻力，使其不失重
            rb.velocity += Vector2.up * Physics2D.gravity.y * upResis * Time.deltaTime;

        /*if (isClimb && !isDash) rb.gravityScale = 0;
        else rb.gravityScale = 1;*/
    }
}
