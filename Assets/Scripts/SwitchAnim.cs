﻿using System.Collections;
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
        anim.SetBool("isGrab", move.isGrab);
        anim.SetBool("canMove", move.canMove);
        anim.SetBool("isDashing", move.isDashing);
    }

    public void basicMove(float x, float y, float vertiVel)  //动画切换
    {
        anim.SetFloat("horiAxis", x);
        anim.SetFloat("vertiAxis", y);
        anim.SetFloat("vertiVel", vertiVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {
        if (move.isGrab)
        {
            if (side == -1 && sr.flipX) return;
            if (side == 1 && !sr.flipX) return;
        }

        bool isFlip = (side == 1) ? false : true;
        sr.flipX = isFlip;
    }
}
