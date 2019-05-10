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
using Leap.Unity;
using Leap.Unity.Infix;
using Leap.Unity.Interaction;
using System;
using System.Reflection;

public static class HandUIExtenstions {

	public static float sqrDist(this Vector3 origin, Vector3 dest){
		return (dest-origin).sqrMagnitude;
	}

	public static float xzSqrDist(this Vector3 origin, Vector3 dest){
		Vector3 a = origin;
		Vector3 b = dest;
		
		a.y = b.y;

		return (a-b).sqrMagnitude;
	}

	public static bool isPinched(this InteractionHand hand)
	{
		return HandUIManager.GetPinchGesture(hand.chirality()).isActive && hand.isTracked;
	}

	public static InteractionHand otherHand(this InteractionHand hand)
	{
		return hand.isLeft ? HandUIManager.handRight : HandUIManager.handLeft;
	}
	public static Vector3 pinchPos(this InteractionHand hand){
		return (hand.indexPos() + hand.thumbPos())/2;
	}
	
	public static Chirality chirality(this InteractionHand hand)
	{
		return hand.isLeft ? Chirality.Left : Chirality.Right;
	}

	public static Vector3 stablePinchPos(this InteractionHand hand)
	{
		return HandUIManager.GetPinchGesture(hand.chirality()).stablePinch.position;
	}

	public static Transform stablePinchTransform(this InteractionHand hand)
	{
		return HandUIManager.GetPinchGesture(hand.chirality()).stablePinch;
	}
	
	public static Vector3 indexPos(this InteractionHand hand){
		return hand.leapHand.GetIndex().TipPosition.ToVector3();
	}

	public static Vector3 thumbPos(this InteractionHand hand)
	{
		return hand.leapHand.GetThumb().TipPosition.ToVector3();
	}

	public static float pinchSqrWidth(this InteractionHand hand, float maxWidth = 0.06f)
	{
		return !hand.isTracked ? maxWidth : hand.indexPos().sqrDist(hand.leapHand.GetThumb().TipPosition.ToVector3());
	}
	
	// How closed is the pinch : fully open 0 .. 1 fully closed
	public static float pinchCloseness(this InteractionHand hand, float maxWidth = 0.06f)
	{
		float activateDistance = 0.02f; //HandUIManager.pinchRight.pinchActivateDistance;
		return Mathf.InverseLerp( activateDistance * activateDistance, maxWidth * maxWidth, hand.pinchSqrWidth());
	}

	public static float sqr(this float value)
	{
		return value * value;
	}

	public static void Follow(this Vector3 position, Vector3 target, float factor = 0.2f)
	{
		position += (target - position) * factor * 90 * Time.deltaTime;
	}
	
	public static float XZSqrDist(this Vector3 origin, Vector3 dest){
		Vector3 a = origin;
		Vector3 b = dest;
		
		a.y = b.y;

		return (a-b).sqrMagnitude;
	}
	
	public static Vector2 Rotate(this Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}

}

