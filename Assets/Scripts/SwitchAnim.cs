using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAnim : MonoBehaviour
{
    private Animator anim;
    private Movement move;
    private CollCheck coll;

    public SpriteRenderer sr;
    void Start()
    {
        anim = GetComponent<Animator>();
        move = GetComponent<Movement>();
        coll = GetComponent<CollCheck>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onRightWall", coll.onRightWall);
        anim.SetBool("wallGrab", move.wallGrab);
        anim.SetBool("canMove", move.canMove);
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
