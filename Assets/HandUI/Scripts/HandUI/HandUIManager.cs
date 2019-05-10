/******************************************************************************
 
 Copyright (C) 2019 Eugene Krivoruchko                                           

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 ******************************************************************************/

using System;
using System.ComponentModel;
using Leap.Unity;
using Leap.Unity.Gestures;
using Leap.Unity.Interaction;
using UnityEngine;
using Object = System.Object;

public class HandUIManager : MonoBehaviour
{
    public enum UIColorMode
    {
        Transparent,
        Gradient
    }

    public static Transform head;

    public static Color whitePoint;

    public static PinchGesture pinchLeft, pinchRight;
    public static InteractionHand handRight, handLeft;
    public static HandRaycast rayLeft, rayRight;

    public static Vector3 handRdelta, handLdelta;
    public static Vector3 headPos;
    public static Vector3 headAngularVelocity = Vector3.zero;

    public static bool shakaL;
    public static bool shakaR;
    public static bool shakaShown;

    public static bool leftHandHolding;
    public static bool rightHandHolding;

    public static Transform heldRObject, heldLObject;

    public static Action OnHeadMoveStart, OnHeadMoveEnd;

    [Header("Global UI color")] [SerializeField]
    private Color _whitePoint = Color.white;
    [SerializeField]
    private Gradient _colorGradient;
    
    private bool headMoving;

    [Range(0, 1)] public float opacity = 1f;

    private Quaternion prevHeadRot;

    private Vector3 prevPosR, prevPosL, angularVelocity;
    private float t;
    
    [SerializeField]
    private UIColorMode _uiColorMode = UIColorMode.Transparent;

    public static UIColorMode uiColorMode;
    public static Gradient colorGradient;
    private void Awake()
    {
        uiColorMode = _uiColorMode;
        whitePoint = _whitePoint;
        colorGradient = _colorGradient;
        
        head = Hands.Provider.transform;

        var pinchGestures = FindObjectsOfType<PinchGesture>();
        if (pinchGestures.Length > 1)
            foreach (var item in pinchGestures)
                if (item.whichHand == Chirality.Left)
                    pinchLeft = item;
                else if (item.whichHand == Chirality.Right) pinchRight = item;
        else
            Warn("Both left and right PinchGestures are required in the scene", this);

        var interactionHands = FindObjectsOfType<InteractionHand>();
        if (interactionHands.Length > 1)
            foreach (var item in interactionHands)
                if (item.isLeft)
                    handLeft = item;
                else
                    handRight = item;
        else
            Warn("Both left and right InteractionHands are required in the scene",this);

        var raycasts = FindObjectsOfType<HandRaycast>();
        if (raycasts.Length > 1)
            foreach (var item in raycasts)
                if (item.whichHand == Chirality.Left)
                    rayLeft = item;
                else if (item.whichHand == Chirality.Right) rayRight = item;
        else
             Warn("Couldn't find both left and right HandRaycasts in the scene",this);

        prevPosR = handRight.leapHand.GetIndex().TipPosition.ToVector3();
        prevPosL = handLeft.leapHand.GetIndex().TipPosition.ToVector3();

        prevHeadRot = head.rotation;
    }

    private bool ShakaIsShown(InteractionHand hand)
    {
        return hand.isTracked && hand.leapHand.GetThumb().IsExtended && hand.leapHand.GetPinky().IsExtended &&
               !hand.leapHand.GetIndex().IsExtended && !hand.leapHand.GetMiddle().IsExtended &&
               !hand.leapHand.GetRing().IsExtended;
    }

    private void Update()
    {
        shakaL = ShakaIsShown(handLeft);
        shakaR = ShakaIsShown(handRight);
        shakaShown = shakaL || shakaR;

        headPos = head.position;

        if (!pinchLeft.isActive) leftHandHolding = false;
        if (!pinchRight.isActive) rightHandHolding = false;

        if (t < 0.2f)
        {
            angularVelocity += CurrentAngVel();
        }
        else
        {
            angularVelocity /= t;
            t = 0;

            headAngularVelocity = angularVelocity / 10;

            angularVelocity = Vector3.zero;

            if (headAngularVelocity.sqrMagnitude > 1f)
            {
                if (!headMoving)
                {
                    headMoving = true;
                    if (OnHeadMoveStart != null) OnHeadMoveStart();
                }
            }
            else
            {
                if (headMoving)
                {
                    headMoving = false;
                    if (OnHeadMoveEnd != null) OnHeadMoveEnd();
                }
            }
        }

        t += Time.deltaTime;
        prevHeadRot = head.rotation;
    }

    private Vector3 CurrentAngVel()
    {
        var deltaRotation = head.rotation * Quaternion.Inverse(prevHeadRot);
        var eulerRotation = new Vector3(
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.x)),
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.y)),
            Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.z)));

        return eulerRotation.Abs() / Time.deltaTime * Mathf.Deg2Rad;
    }

    private void LateUpdate()
    {
        var posR = handRight.leapHand.GetIndex().TipPosition.ToVector3();
        var posL = handLeft.leapHand.GetIndex().TipPosition.ToVector3();

        handRdelta = posR - prevPosR;
        handLdelta = posL - prevPosL;

        prevPosR = posR;
        prevPosL = posL;
    }

    public static InteractionHand GetHand(Chirality chirality)
    {
        return chirality == Chirality.Left ? handLeft : handRight;
    }

    public static PinchGesture GetPinchGesture(Chirality chirality)
    {
        return chirality == Chirality.Left ? pinchLeft : pinchRight;
    }
    
    public static HandRaycast GetRaycast(Chirality chirality)
    {
        return chirality == Chirality.Left ? rayLeft : rayRight;
    }

    
    public static Color GetColor(float a, Color? overrideColor = null)
    {
        var c = whitePoint;
        
        if (overrideColor == null)
        {
            switch (uiColorMode)
            {
                case UIColorMode.Transparent:
                    c.a = a;
                    return c;
                case UIColorMode.Gradient:
                    c = colorGradient.Evaluate(a);
                    return c;
            }
        }
        else
        {
            c = overrideColor ?? default(Color);
            c.a = a;
            return c;
        }

        return Color.magenta;
    }

    public static Vector3 PostitionInFrontOfHead(float distance, float yOffset = 0)
    {
        return head.position + XZLookDirection() * distance + yOffset * Vector3.up;
    }

    public static Vector3 XZLookDirection()
    {
        Vector3 lookDirXZ = head.forward;
        lookDirXZ.y = 0;
        lookDirXZ.Normalize();
        return lookDirXZ;
    }
    
    public static void Warn(string msg, UnityEngine.Object obj)
    {
        Debug.Log("<b>HandUI: </b>" + msg, obj);
    }
        
}