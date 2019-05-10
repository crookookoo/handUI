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
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Events;
using Pose = Leap.Unity.Pose;

public class HandUIObject : MonoBehaviour
{
    public ObjectType objectType = ObjectType.Point;
    public float hoverDistance = 0.1f;

    [Tooltip("Trigger objects won't prevent other objects to be hovered or grabbed while")]
    public bool isTrigger = false;

    public enum ObjectType
    {
        Rect,
        Line,
        Point,
        Circle
    }

    public enum PointerMode
    {
        IndexTip,
        PinchPosition,
        OverridePointer
    }


    [HideInInspector] public static HandUIManager handUIManager;

    public static bool oneHovered;

    public static bool canBeHovered = true;

    private Transform _anchor;

    public Vector2 buttonSize;
    public Vector3 buttonPivot;

    public Transform lineStart, lineEnd;

    public float circleRadius;

    [Space] public PointerMode pointerMode = PointerMode.IndexTip;
    public Transform pointerOverrideL, pointerOverrideR;

    [HideInInspector] public InteractionHand closestHand;
    [HideInInspector] public float closestHandDist = float.PositiveInfinity;
    [HideInInspector] public float closestHandLineDist = float.PositiveInfinity;
    [HideInInspector] public float closestHandRectDist = float.PositiveInfinity;

    private Vector3 closestHandPointer, prevPos, posDelta, localPosDelta, prevMidpointPos, prevDeltaV;

    [HideInInspector] public float closestHandXZDist = float.PositiveInfinity;
    [HideInInspector] public float deltaAngle;

    private float dL, dR;

    [HideInInspector] public bool handIsClose;

    private InteractionHand handR, handL;

    [HideInInspector] public bool hovered;

    [HideInInspector] public bool isDragged;

    private LineRenderer lineRenderer;

    private float maxScale = 0.5f;

    private float minScale = 0.06f;


    public bool moveWithPinch;
    public bool twoHandedTRS;

    private float onGrabDistanceBetweenHands, prevAngle;
    private Vector3 onGrabScale, onGrabEulerAngles, onGrabRightToLeft, onGrabMidpointOffset;

    public event Action OnHoverBegin, OnHoverEnd, OnPinchBegin, OnPinchEnd;
    public UnityEvent OnHoverEvent, OnUnhoverEvent;

    [HideInInspector] public bool pinched;

    private bool bothHandsPinched = false;
    
    private Vector3 prevPinchPos = Vector3.zero;
    public Vector3 currentDeltaPinchPos = Vector3.zero;
    private Vector3 initPositionWhenPinched;
    public Vector3 distanceFromInitPinchPos;

    private Vector3 initButtonSize;
    private bool reset = false;

    [HideInInspector] public float dist = float.PositiveInfinity;

    private void Start()
    {

        if (handUIManager == null)
        {
            var foundObjects = FindObjectsOfType<HandUIManager>();
            if (foundObjects.Length > 0) handUIManager = foundObjects[0];
            else Debug.LogError("HandUIManager is missing in the scene.");
        }

        if (objectType == ObjectType.Line)
            lineRenderer = GetComponent<LineRenderer>();

        if (objectType == ObjectType.Rect)
        {
            initButtonSize = buttonSize;
        }

        handL = HandUIManager.handLeft;
        handR = HandUIManager.handRight;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if (objectType == ObjectType.Point)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(transform.position, hoverDistance);
        }

        if (objectType == ObjectType.Rect)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 cubeSize = buttonSize;
            cubeSize.z = 0;
            Gizmos.DrawWireCube(Vector3.zero + buttonPivot, cubeSize);
            
            Color c = Color.magenta;
            c.a = 0.2f;
            Gizmos.color = c;
            
            cubeSize.z = 2 * hoverDistance;
            Gizmos.DrawWireCube(Vector3.zero + buttonPivot, cubeSize);

        }

        if (objectType == ObjectType.Circle)
        {
            DrawCircle((circleRadius) * transform.lossyScale.x);

            Color c = Color.magenta;
            c.a = 0.2f;
            Gizmos.color = c;

            DrawCircle(circleRadius * transform.lossyScale.x + hoverDistance / 2);

            if (circleRadius * transform.lossyScale.x - hoverDistance / 2 > 0)
                DrawCircle(circleRadius * transform.lossyScale.x - hoverDistance / 2);

        }
        
        
    }

    private void DrawCircle(float radius)
    {
        float theta = 0;

        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);

        Vector3 pos = transform.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;

        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }

        Gizmos.DrawLine(pos, lastPos);
    }

    public bool bothHandsClose()
    {
        return isHandClose(HandUIManager.handLeft) && isHandClose(HandUIManager.handLeft);
    }

    public bool isHandClose(InteractionHand hand)
    {
        if (hand.isTracked)
        {
            float dist = hand.isLeft ? dL : dR;

            if (dist < hoverDistance * hoverDistance)
                return true;
        }

        return false;
    }

    private Vector3 closestPointOnCircle(Vector3 circleCenter, float radius, Vector3 target)
    {
        Vector3 XZPos = target;
        XZPos.y = circleCenter.y;

        Vector3 direction = (XZPos - circleCenter).normalized;

        return circleCenter + direction * radius;

    }

    public Vector3 ClosestPointOnCircleToHand(InteractionHand hand)
    {
        return closestPointOnCircle(transform.position, circleRadius * transform.lossyScale.x, HandPointerPos(hand));
    }

    // distance to hand in world scale
    private float HandDistance(InteractionHand hand)
    {
        if (!hand.isTracked) return float.PositiveInfinity;

        var lossyScale = transform.lossyScale.x;

        switch (objectType)
        {
            case ObjectType.Point:
                return transform.position.sqrDist(HandPointerPos(hand));
            case ObjectType.Line:
                return DistToLineSegment(HandPointerPos(hand), lineRenderer);
            case ObjectType.Circle:
                return ClosestPointOnCircleToHand(hand).sqrDist(HandPointerPos(hand));
            case ObjectType.Rect:
                Vector3 pointerInObjSpace;
                pointerInObjSpace = (transform.InverseTransformPoint(HandPointerPos(hand)) - buttonPivot) * lossyScale;
                Vector3 worldButtonSize = buttonSize * lossyScale;

                if (Mathf.Abs(pointerInObjSpace.x) < worldButtonSize.x / 2 &&
                    Mathf.Abs(pointerInObjSpace.y) < worldButtonSize.y / 2 &&
                    Mathf.Abs(pointerInObjSpace.z) < hoverDistance)
                    return pointerInObjSpace.z * pointerInObjSpace.z;
                else
                    return float.PositiveInfinity;
        }

        return float.PositiveInfinity;

    }
    
    public Vector3 HandPointerPos(InteractionHand hand)
    {
        switch (pointerMode)
        {
            case PointerMode.OverridePointer when !hand.isTracked:
                return Vector3.positiveInfinity;
            case PointerMode.OverridePointer:
                return hand.isLeft ? pointerOverrideL.position : pointerOverrideR.position;
            case PointerMode.IndexTip:
                return hand.indexPos();
            case PointerMode.PinchPosition:
                return hand.pinchPos();
            default:
                return hand.leapHand.PalmPosition.ToVector3();
        }
    }

    private void Update()
    {
        closestHand = null;

        //find closest hand
        dL = HandDistance(HandUIManager.handLeft);
        dR = HandDistance(HandUIManager.handRight);

        if (dL <= dR) closestHand = HandUIManager.handLeft;
        else if (dR < dL) closestHand = HandUIManager.handRight;

        closestHandDist = Mathf.Min(dL, dR);
        
        dist = closestHandDist;
        handIsClose = dist < hoverDistance * hoverDistance;

        if (handIsClose && !hovered && canBeHovered && !isClosestPinching())
        {
            BeginHover();
        }

        if (!handIsClose && !pinched && hovered)
        {
            EndHover();
        }

        reset = false;

        if (hovered && !pinched && isClosestPinching() && !isClosestHandHolding())
        {
            BeginPinch();
        }

        if (pinched && !isClosestPinching())
        {
            EndPinch();
        }
        
        if(!bothHandsPinched && pinched && closestHand.otherHand().isPinched() && isHandClose(closestHand.otherHand()))
        {
            // start double pinch
            bothHandsPinched = true;

            onGrabDistanceBetweenHands = Vector3.Distance(HandUIManager.pinchLeft.pose.position,
                HandUIManager.pinchRight.pose.position);
            onGrabScale = transform.localScale;
            onGrabEulerAngles = transform.eulerAngles;

            var midpoint = (HandUIManager.pinchLeft.pose.position + HandUIManager.pinchLeft.pose.position) / 2;
            prevMidpointPos = midpoint;
            deltaAngle = 0;
            reset = true;

            onGrabMidpointOffset = midpoint - transform.position;
            onGrabRightToLeft = HandUIManager.pinchLeft.pose.position - HandUIManager.pinchRight.pose.position;
            onGrabRightToLeft.y = 0;

        }

        if (bothHandsPinched && !(closestHand.isPinched() && closestHand.otherHand().isPinched()))
        {
            //end double pinch
            bothHandsPinched = false;
            onGrabMidpointOffset = HandPointerPos(closestHand) - transform.position;

        }

        if (pinched)
        {
            var currentPinchPos = HandPointerPos(closestHand);
            currentDeltaPinchPos = currentPinchPos - prevPinchPos;
            distanceFromInitPinchPos = currentPinchPos - initPositionWhenPinched;
            prevPinchPos = currentPinchPos;
        }

        if (pinched && moveWithPinch)
        {

            if (twoHandedTRS && bothHandsPinched)
            {
                // TRS with 2 hands
                var midpoint = (HandUIManager.pinchLeft.pose.position + HandUIManager.pinchLeft.pose.position) / 2;

                var currentDistance = Vector3.Distance(HandUIManager.pinchLeft.pose.position,
                    HandUIManager.pinchRight.pose.position);
                var currentRightToLeft = HandUIManager.pinchLeft.pose.position - HandUIManager.pinchRight.pose.position;
                currentRightToLeft.y = 0;

                var angle = Vector3.SignedAngle(currentRightToLeft, onGrabRightToLeft, Vector3.up);
                if (!reset) deltaAngle = prevAngle - angle;
                else reset = false;

                prevAngle = angle;
                onGrabMidpointOffset = Quaternion.Euler(0, deltaAngle, 0) * onGrabMidpointOffset;
                transform.Rotate(deltaAngle * Vector3.up);
                
                transform.position = midpoint - onGrabMidpointOffset * currentDistance / onGrabDistanceBetweenHands;
                transform.localScale = onGrabScale * currentDistance / onGrabDistanceBetweenHands;

            } 
            else
            {
                // move with one hand
                transform.position = HandPointerPos(closestHand) - onGrabMidpointOffset;
            }
            

        }
        
    }

    private void BeginHover()
    {
        hovered = true;
        oneHovered = true;

        if (objectType == ObjectType.Rect)
        {
            buttonSize = initButtonSize * 1.1f;
        }


        if (OnHoverBegin != null) OnHoverBegin();

        OnHoverEvent.Invoke();

    }

    private void EndHover()
    {
        hovered = false;
        oneHovered = false;

        if (objectType == ObjectType.Rect)
        {
            buttonSize = initButtonSize;
        }

        if (OnHoverEnd != null) OnHoverEnd();

        OnUnhoverEvent.Invoke();


    }

    private void BeginPinch()
    {
        pinched = true;
        if (OnPinchBegin != null) OnPinchBegin();

        prevPinchPos = HandPointerPos(closestHand);
        initPositionWhenPinched = prevPinchPos;

        onGrabMidpointOffset = HandPointerPos(closestHand) - transform.position;

        if (closestHand != null && !isTrigger) setHandHoldingState(closestHand, true);

    }

    private void EndPinch()
    {
        deltaAngle = 0;

        distanceFromInitPinchPos = Vector3.zero;

        pinched = false;
        if (closestHand != null) setHandHoldingState(closestHand, false);
        if (OnPinchEnd != null) OnPinchEnd();


    }

    private void LateUpdate()
    {
        posDelta = closestHandPointer - prevPos;
        localPosDelta = transform.InverseTransformPoint(closestHandPointer) - transform.InverseTransformPoint(prevPos);

        prevPos = closestHandPointer;
    }

    public Vector3 handPosDelta()
    {
        return posDelta;
    }

    public Vector3 handPosDeltaLocal()
    {
        return localPosDelta;
    }

    private bool isClosestPinching()
    {
        return closestHand != null &&
               (closestHand.isLeft ? HandUIManager.pinchLeft.isActive : HandUIManager.pinchRight.isActive);
    }

    private bool isClosestHandHolding()
    {
        if (closestHand == null) Debug.LogWarning("Closest hand is null");
        return closestHand != null && closestHand.isLeft
            ? HandUIManager.leftHandHolding
            : HandUIManager.rightHandHolding;
    }

    private void setHandHoldingState(InteractionHand hand, bool value)
    {
        if (hand.isLeft) HandUIManager.leftHandHolding = value;
        if (hand.isRight) HandUIManager.rightHandHolding = value;
    }

    private void setBothHandsHoldingState(bool value)
    {
        HandUIManager.leftHandHolding = value;
        HandUIManager.rightHandHolding = value;
    }

    public Vector3 closestPinchedHandPos()
    {
        return closestHand.pinchPos();
    }

    public HandRaycast closestHandRaycast()
    {
        if (closestHand != null)
            return closestHand.isLeft ? HandUIManager.rayLeft : HandUIManager.rayRight;
        return null;
    }

    private float DistToLineSegment(Vector3 point, LineRenderer line)
    {
        var closestPoint = ClosestPointOnLineSegment(point, lineStart.position, lineEnd.position);

        return point.sqrDist(closestPoint);
    }

    private Vector3 ClosestPointOnLineSegment(Vector3 rPoint, Vector3 rLineStart, Vector3 rLineEnd)
    {
        var lLine = rLineEnd - rLineStart;

        var lProject = Vector3.Project(rPoint - rLineStart, lLine);
        var lAxisPoint = lProject + rLineStart;

        if (Vector3.Dot(lProject, lLine) < 0f)
            lAxisPoint = rLineStart;
        else if (lProject.sqrMagnitude > lLine.sqrMagnitude) lAxisPoint = rLineEnd;

        return lAxisPoint;
    }
    
    private float MaxAbs(float a, float b) {
        if (Mathf.Abs(a) >= Mathf.Abs(b)) {
            return a;
        }
        return b;
    }

}