// Copyright (c) 2021 Kourosh T. Baghaei
//
// This source code is licensed under the license found in the
// LICENSE file in the root directory of this source tree.



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class BatController : MonoBehaviour
{
    public Vector2 speed = Vector2.left;
    public AudioClip birdScream;
    private Animator batAnimator;
    private Rigidbody2D batRigidbody2D;
    private Collider2D col2d;
    private AudioSource audioSource;
    void Start()
    {
        batAnimator = GetComponentInChildren<Animator>();
        batRigidbody2D = GetComponentInChildren<Rigidbody2D>();
        col2d = GetComponentInChildren<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        batRigidbody2D.velocity = speed;
    }

    public bool debugOnlyDieIn3Seconds = false;
    private bool debugOnlyDeathActivated = false;
    private float debugOnlyDieStartTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (debugOnlyDieIn3Seconds && !debugOnlyDeathActivated)
        {
            debugOnlyDieStartTime = Time.time;
            debugOnlyDeathActivated = true;
        }
        
        if(debugOnlyDeathActivated)
        {
            if (Time.time - debugOnlyDieStartTime >= 3)
            {
                
                //KillTheBat(new Vector2(10f, 5f));
                debugOnlyDeathActivated = false;
            }
        }
    }

    void KillTheBat(Vector2[] args)
    {
        //Debug.LogWarning(args);
        Vector2 hitPoint = (Vector2) args[0];
        Vector2 imuplseForce = (Vector2) args[1];
        col2d.enabled = false;
        batRigidbody2D.gravityScale = 2.0f;
        batRigidbody2D.AddForceAtPosition(imuplseForce,hitPoint, ForceMode2D.Impulse);
        batAnimator.SetBool("die",true);
        GameObject.Destroy(this.gameObject, 5.0f);
        audioSource.PlayOneShot(birdScream);
    }
}
