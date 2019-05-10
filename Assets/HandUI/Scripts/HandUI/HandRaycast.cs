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
using Leap.Unity;
using Leap.Unity.Gestures;
using Leap.Unity.Interaction;
using UnityEngine;

/// <summary>
/// The HandModelManager manages a pool of HandModelBases and makes HandRepresentations
/// </summary>


public class HandRaycast : MonoBehaviour
{
    
    [HideInInspector] public Transform hoveredTransform;

    [HideInInspector] public bool hovering;

    private HandRaycastTarget hrt;
    public LayerMask layermask;

    public Action OnHoverBegin, OnHoverEnd, OnPinchBegin, OnPinchEnd, OnPinchClick, OnAirTap;

    private PinchGesture pg;

    [HideInInspector] public bool pinching;
    [HideInInspector] public Vector3 direction;

    private bool tapping;
    private readonly float threshold = 0.02f;
    private float timeSinceLastTap;

    public Chirality whichHand;
    
    [HideInInspector] public HandRaycastTarget currentTarget;
    
    private Vector3 fPos, fPrevPos, fVel, wPos, wPrevPos, wVel, projectedVel, crossVel;
    private InteractionHand hand;
    private Transform head;
    
    private bool targetChanged = false;
    public float shoulderWidth = 0.15f;
    private Vector3 prevPinchPos, currentDragDelta, onDragPinchPos;
    private float lastPinchTravel = 0;
    private float travelThreshold = 0.0001f;
    private void Start()
    {
        head = HandUIManager.head;
        
        hand = HandUIManager.GetHand(whichHand);
        pg = HandUIManager.GetPinchGesture(whichHand);

        // AirTap init, getting index and wrist postions
        fPos = hand.leapHand.GetIndex().TipPosition.ToVector3();
        fPrevPos = fPos;

        wPos = hand.leapHand.WristPosition.ToVector3();
        wPrevPos = wPos;
        
    }
    private void Update()
    {

        var shoulderPosition = hand.isLeft
            ? head.TransformPoint(new Vector3(-shoulderWidth, -.13f, 0f))
            : head.TransformPoint(new Vector3(shoulderWidth, -.13f, 0f));
        
        direction = (hand.stablePinchPos() - shoulderPosition).normalized;

        var ray = new Ray(shoulderPosition, direction);

        RaycastHit hit;

        WatchAirtap();

        if (hand.isTracked && Physics.Raycast(ray, out hit, 10, layermask))
        {
            targetChanged = hovering && hit.collider.transform != hoveredTransform && !pinching;
            
            if ((!hovering || targetChanged) && hit.collider != null)
            {
                if (targetChanged)
                {
                    currentTarget.SetHit(whichHand, Vector3.zero, false);
                    if(currentTarget.OnHoverEnd != null && !currentTarget.isHovered) currentTarget.OnHoverEnd();                    
                }

                currentTarget = hit.collider.gameObject.GetComponent<HandRaycastTarget>();
                currentTarget.hoveringRaycast = this;
                
                if (currentTarget != null)
                {
                    if (!currentTarget.isHovered)
                    {
                        currentTarget.SetHit(whichHand, hit.point, true);
                        if(currentTarget.OnHoverBegin != null && !currentTarget.isHovered) currentTarget.OnHoverBegin();
                    }

                    hoveredTransform = hit.collider.transform;
                }

                if (OnHoverBegin != null) OnHoverBegin();
                hovering = true;
            }

            if (hovering && currentTarget != null)
            {
                currentTarget.SetHit(whichHand, hit.point, true);
            }

            if (!pinching && pg.isActive)
            {
                pinching = true;
                if (OnPinchBegin != null) OnPinchBegin();
                if(currentTarget.OnPinchBegin != null) currentTarget.OnPinchBegin();
                
                lastPinchTravel = 0;
                prevPinchPos = hand.pinchPos();
                onDragPinchPos = hand.pinchPos();
                currentDragDelta = Vector3.zero;
            }

            if (tapping)
            {
                if (OnAirTap != null) OnAirTap();
                if(currentTarget.OnAirTap != null) currentTarget.OnAirTap();

            }
        }
        else
        {
            if (hovering && !pinching)
            {
                if (OnHoverEnd != null) OnHoverEnd();
                if (currentTarget != null)
                {
                    currentTarget.SetHit(whichHand, Vector3.zero, false);
                    if(currentTarget.OnHoverEnd != null && !currentTarget.isHovered) currentTarget.OnHoverEnd();
                }

                hoveredTransform = null;
                hovering = false;
            }
        }


        if (pinching)
        {
            lastPinchTravel += hand.pinchPos().sqrDist(prevPinchPos);
            currentDragDelta = hand.pinchPos() - onDragPinchPos;
        }
        
        if (pinching && !pg.isActive)
        {
            pinching = false;
            if (OnPinchEnd != null) OnPinchEnd();
            if(currentTarget.OnPinchEnd != null) currentTarget.OnPinchEnd();
           
            if (lastPinchTravel < travelThreshold)
            {
                if(currentTarget.OnPinchClick != null) currentTarget.OnPinchClick();
                if (OnPinchClick != null) OnPinchClick();
//                Debug.Log("click! travel: " + lastPinchTravel);
            }
        }

        tapping = false;
        prevPinchPos = hand.pinchPos();
    }

    public Vector3 GetCurrentDragDelta()
    {
        return currentDragDelta;
    }
    
    private void WatchAirtap()
    {
        if (hand.isTracked)
        {
            fPos = hand.leapHand.GetIndex().TipPosition.ToVector3();
            fVel = fPos - fPrevPos;
            fPrevPos = fPos;

            wPos = hand.leapHand.WristPosition.ToVector3();
            wVel = wPos - wPrevPos;
            wPrevPos = wPos;

            var mainAxis = (hand.leapHand.WristPosition.ToVector3() - head.position).normalized;
            var dotProduct = Vector3.Dot(fVel - wVel, mainAxis);

            if (fPos.y > hand.leapHand.PalmPosition.ToVector3().y && dotProduct > threshold && timeSinceLastTap > 0.2f)
            {
                tapping = true;
                timeSinceLastTap = 0;
            }
        }

        if (timeSinceLastTap < 0.6f) timeSinceLastTap += Time.deltaTime;
    }
}