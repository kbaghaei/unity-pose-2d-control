// Copyright (c) 2021 Kourosh T. Baghaei
//
// This source code is licensed under the license found in the
// LICENSE file in the root directory of this source tree.


using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using MiscUtil.Collections.Extensions;
using UnityEngine;

public class AvatarController2D : MonoBehaviour
{
    /// <summary>
    /// Based on the character's movement state, the camera leans toward left or right
    /// To open more space for the player to see in front of the character.
    /// </summary>
    public enum CharacterMovementState
    {
        LeftWards,
        Idle,
        RightWards
    }

    public CharacterMovementState currentMovementState = CharacterMovementState.Idle;
    
    private PythonPortal pythonPortal = null;
    private PythonPlugger pythonPlugger = null;
    private int latestFrameIndex = -1;
    private InputStablizer inputStablizer = InputStablizer.mainStablizer;
    void Start()
    {
        // Through pre-defined order of code execution we make sure pythonPortal is initialized first.
        // Then we assign it to local variable here:
        //pythonPortal = PythonPortal.mainPortal();
        
        // Through pre-defined order of code execution we make sure pythonPlugger is initialized first.
        // Then we assign it to local variable here:
        pythonPlugger = PythonPlugger.MainPlugger();

        if (cameraLeftTarget == null || cameraRightTarget == null)
        {
            throw new UnassignedReferenceException("Camera targets must be set.");
        }

        if (theMainCamera == null)
        {
            throw new UnassignedReferenceException("The main camera must be set in avatar controller.");
        }

        if (characterOnTheScene == null)
        {
            throw new UnassignedReferenceException("Set the character to Avatar Controller!");
        }

        if (gunsAndHands == null)
        {
            throw new UnassignedReferenceException("Set the Guns And Hands of the character");
        }

        if (gunShotFireAnimator == null)
        {
            throw new UnassignedReferenceException("Gun shot animator must be set!");
        }
        
        characterAnimator = characterOnTheScene.GetComponent<Animator>();
        characterTransform = characterOnTheScene.GetComponent<Transform>();
        characterRigidBody2D = characterOnTheScene.GetComponent<Rigidbody2D>();
        barrelHead = gunShotFireAnimator.GetComponent<Transform>();
        theGunSound = this.GetComponent<AudioSource>();
    }
    
    public GameObject characterOnTheScene;
    public Transform cameraLeftTarget;
    public Transform cameraRightTarget;
    public Camera theMainCamera;
    public Transform gunsAndHands;

    public Animator gunShotFireAnimator;
    public AudioClip gunshot;

    private Transform barrelHead;
    private Animator characterAnimator;
    private Transform characterTransform;
    private Rigidbody2D characterRigidBody2D;

    public float runSpeed = 2.0f;

    public float gunPower = 5.0f;
    // Info updated from the vision inputs
    private Vector3 headPosFromVision = Vector3.zero;
    private Vector3 leftHandPosFromVision = Vector3.zero;
    private Vector3 rightHandPosFromVision = Vector3.zero;
    private Vector3 handsDiffVect = Vector3.right;
    private Vector3 gunDirection;
    
    /// <summary>
    /// Radius Around Center Threshold:
    /// Considering the screen's width from 0 to 100, the center would be 50%.
    /// The radius around that which by default is 15% means that the center of the screen
    /// is from 50 - 15 to 50 + 15 percentages. If the head of the player is located in these areas, the character will stay idle.
    /// Otherwise, it will move to left or right.
    /// </summary>
    public float radiusAroundCenterThreshold = 0.15f;

    /// <summary>
    /// The distance between the two hands of the player imply aiming a gun.
    /// If the hands get closer than a certain threshold, it is considered
    /// as pulling the trigger.
    /// </summary>
    public float triggerTreshold = 0.3f;
    
    public float reloadTime = 0.2f;
    private float reloadCooldown = 0.2f;
    private bool shotBullet = false;

    private AudioSource theGunSound;
    
    public float debugOnlyHeadPosition = 0.0f;
    public Vector2 debugOnlyLeftRightDiff = Vector2.one * 5;
    public float debugOnlyHandsDistance = 0.0f;

    public Vector3 debugOnlyHeadPosVis;
    public Vector3 debugOnlyLeftHandPosVis;
    public Vector3 debugOnlyRightHandPosVis;
    private int debugOnlyBulletsShot = 0;
    private bool DEBUGING = false;
    void Update()
    {
        #region KeyPointsPack2D From Python Portal

        KeyPointsPack2D kp2d = pythonPlugger.getLatestPack2D();
        if (kp2d != null)
        {
            // do not update info based on an old frame.
            if (kp2d.frame_in_seq > latestFrameIndex)
            {
                latestFrameIndex = kp2d.frame_in_seq;
                inputStablizer.UpdateKeyPointsPack2D(kp2d);

                headPosFromVision = inputStablizer.GetHeadPos();
                leftHandPosFromVision = inputStablizer.GetLeftHandPos();
                rightHandPosFromVision = inputStablizer.GetRightHandPos();

                debugOnlyHeadPosVis = headPosFromVision;
                debugOnlyLeftHandPosVis = leftHandPosFromVision;
                debugOnlyRightHandPosVis = rightHandPosFromVision;

                handsDiffVect = leftHandPosFromVision - rightHandPosFromVision;
                gunDirection = barrelHead.right;

            }

        }

        #endregion
            
        // Update Current Movement State
        #region Update Current Movement State
        
        if (0.5f - radiusAroundCenterThreshold <= headPosFromVision.x &&
            headPosFromVision.x <= 0.5f + radiusAroundCenterThreshold)
        {
            currentMovementState = CharacterMovementState.Idle;
            characterRigidBody2D.velocity = Vector2.zero;
        }
        else if (headPosFromVision.x < 0.5f - radiusAroundCenterThreshold)
        {
            currentMovementState = CharacterMovementState.RightWards;
            Vector3 scale = characterTransform.localScale;
            scale.x = 1;
            characterTransform.localScale = scale;
            characterRigidBody2D.velocity = new Vector2(runSpeed, characterRigidBody2D.velocity.y);
            gunDirection = barrelHead.right;
        }
        else if (headPosFromVision.x > 0.5f + radiusAroundCenterThreshold)
        {
            currentMovementState = CharacterMovementState.LeftWards;
            Vector3 scale = characterTransform.localScale;
            scale.x = -1;
            characterTransform.localScale = scale;
            characterRigidBody2D.velocity = new Vector2(-runSpeed, characterRigidBody2D.velocity.y);
            gunDirection = -barrelHead.right;
        }
        
        #endregion

        // Update hands and gun

        #region  Update Hands and Gun

        gunsAndHands.eulerAngles = (-Mathf.Rad2Deg *
                                   Mathf.Atan2(handsDiffVect.y, handsDiffVect.x) + 180.0f) * Vector3.forward;
        
        debugOnlyHandsDistance = handsDiffVect.magnitude;

        if (!shotBullet)
        {
            if (handsDiffVect.magnitude < triggerTreshold)
            {
                shotBullet = true;
                reloadCooldown = reloadTime;
                debugOnlyBulletsShot++;
                theGunSound.PlayOneShot(gunshot);
                gunShotFireAnimator.Play("GunFire");
                RaycastHit2D hit = Physics2D.Raycast(barrelHead.position, gunDirection);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("bat"))
                    {
                        hit.collider.SendMessageUpwards("KillTheBat", new Vector2[] {hit.point, gunDirection * gunPower});    
                    }

                    if (hit.collider.CompareTag("alienship"))
                    {
                        Debug.LogWarning("Maaaaaah");
                        hit.collider.SendMessageUpwards("takeDamage", new Vector2[] {hit.point, gunDirection * gunPower}); 
                    }
                }
                Debug.Log("Do some raycasts! " + debugOnlyBulletsShot.ToString());
                Debug.DrawRay(barrelHead.position, gunDirection * 5, Color.red, 0.2f);
            }
        }
        
            
        // update gun fire's info
        reloadCooldown -= Time.deltaTime;
        if (reloadCooldown <= 0)
        {
            reloadCooldown = reloadTime;
            shotBullet = false;
        }

        #endregion
        
        
        // Update Character's Behavior and Camera
        #region Update Character's Behavior
        Vector3 cameraTarget = Vector3.zero;
        switch (currentMovementState)
        {
            case CharacterMovementState.Idle:
            {
                characterAnimator.SetBool("running", false);
                cameraTarget = (cameraLeftTarget.position + cameraRightTarget.position) * 0.5f;
                break;
            }
            case CharacterMovementState.LeftWards:
            {
                characterAnimator.SetBool("running", true);
                cameraTarget = cameraLeftTarget.position;
                break;
            }
            case CharacterMovementState.RightWards:
            {
                characterAnimator.SetBool("running", true);
                cameraTarget = cameraRightTarget.position;
                break;
            }
        }
        
        Vector2 newCamPos =
            Vector2.Lerp(theMainCamera.transform.position, cameraTarget, Time.deltaTime);
        theMainCamera.transform.position =
            new Vector3(newCamPos.x, newCamPos.y, theMainCamera.transform.position.z);
        
        #endregion
    }
}
