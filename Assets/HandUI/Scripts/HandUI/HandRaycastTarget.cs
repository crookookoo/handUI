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

using UnityEngine;
using System;
using Leap.Unity;
using Leap.Unity.Interaction;

public class HandRaycastTarget : MonoBehaviour {

	[HideInInspector] public bool isHovered = false;
	[HideInInspector] public bool isDoubleHovered = false;
	[HideInInspector] public Vector3 leftHitPosition, rightHitPosition;
	[HideInInspector] public HandRayHit rightHit, leftHit;
	
	public Action OnHoverBegin, OnHoverEnd, OnPinchBegin, OnPinchEnd, OnAirTap, OnPinchClick;

	[HideInInspector] public HandRaycast hoveringRaycast;
	
	public struct HandRayHit
	{
		public Vector3 point;
		public bool isHandHovering;
		
		
		public HandRayHit(Vector3 point, bool isHandHovering)
		{
			this.point = point;
			this.isHandHovering = isHandHovering;
		}
		
	}
	
	public void SetHit(Chirality whichHand, Vector3 point, bool isHandHovering)
	{
		if (whichHand == Chirality.Left)
		{
			leftHit = new HandRayHit(point, isHandHovering);
		}
		else
		{
			rightHit = new HandRayHit(point, isHandHovering);
		}

		if (rightHit.isHandHovering && leftHit.isHandHovering) isDoubleHovered = true;

		if (rightHit.isHandHovering && leftHit.isHandHovering) isHovered = true;

		if (!rightHit.isHandHovering && !leftHit.isHandHovering) isHovered = false;
	}
	
	public HandRayHit GetHit(Chirality chirality)
	{
		return chirality == Chirality.Left ? leftHit : rightHit;
	}

	public Chirality WhichHandHovering()
	{
		return rightHit.isHandHovering ? Chirality.Right : Chirality.Left;
	}
	
	
}
