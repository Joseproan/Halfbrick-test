using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    internal bool onTrampoline;
    public Animator animator;
    public bool startJump;
    private void Update()
    {
        if (onTrampoline)
        {
           onTrampoline = false;
            animator.SetTrigger("On");
        }
    }

    

}
