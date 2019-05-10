using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using Leap.Unity.Attributes;
using Leap.Unity.Gestures;
using DG.Tweening;

public class PinchSphere : MonoBehaviour {

	public Chirality whichHand;

	private InteractionHand hand;
	private PinchGesture pg;

	[HideInInspector]
	public bool followPinch = true;

	[HideInInspector]
	public bool dropped = false;

	private bool hidden = false;
	private bool bright = true;

	[HideInInspector]
	public Vector3 target = Vector3.zero;

	[HideInInspector]
	public Transform targetTransform;	

	private Vector3 initialScale;
	private MeshRenderer mr;
	Vector3 _vposition;
	private float springSpeed = 100f;	

	private bool scaledDown = false;

	private HandUIManager HandUIManager;

	void Start(){
		mr = GetComponent<MeshRenderer>();
		target = transform.position;
		initialScale = transform.localScale;

		if(HandUIManager == null){
			var foundObjects = FindObjectsOfType<HandUIManager>();
			if(foundObjects.Length > 0) HandUIManager = foundObjects[0];
			else Debug.LogError("HandUIManager is missing in the scene.");
		}
		
		if(whichHand == Chirality.Left){
			hand = HandUIManager.handLeft;
			pg = HandUIManager.pinchLeft;
		} else {
			hand = HandUIManager.handRight;
			pg = HandUIManager.pinchRight;
		}

	}

	void LateUpdate () {

		if(dropped){
			float dist = targetTransform.position.sqrDist(hand.leapHand.GetPredictedPinchPosition());	
		
			if(dist < 0.02f) followPinch = true;
			else			followPinch = false;
			
		}

		if(hand.isTracked && followPinch){
			target = 0.5f * (hand.leapHand.GetPredictedPinchPosition());
		}

		if(dropped && !followPinch)
			target = targetTransform.position;

		if(followPinch && !bright)
			Brighten();

		if(!followPinch && bright)
			Dim();

		// if(!hand.isTracked && followPinch && !hidden){
		// 	hidden = true;
		// 	ScaleZero();
		// }

		// if(hidden && hand.isTracked && followPinch){
		// 	hidden = false;
		// 	ScaleDefault();
		// }


		// transform.position = SpringPosition(transform.position, target);
		if(hand.isTracked) transform.position = hand.pinchPos();

		if(pg != null && pg.isActive && !scaledDown){
			ScaleZero();
			scaledDown = true;
		}

		if(pg != null && !pg.isActive && scaledDown){
			ScaleDefault();
			scaledDown = false;
		}

	}
	
	public void ScaleZero(){
		hidden = true;
		transform.DOScale(Vector3.zero, 0.1f);				
	}


	public void Brighten(){
		bright = true;
		mr.material.DOColor(Color.white, 0.1f);
		mr.material.SetColor("_Emission", Color.gray);		
		
		ScaleUp();
	}

	public void Dim(){
		bright = false;
		mr.material.DOColor(Color.clear, 0.1f);
		mr.material.SetColor("_Emission", Color.black);		
		
		ScaleDown();
	}

	public void ScaleDown(){
		// transform.DOScale(initialScale * 0.6f, 0.1f);				
	}

	public void ScaleDefault(){
		transform.DOScale(initialScale, 0.1f);
	}

	public void ScaleUp(){
		// transform.DOScale(initialScale*1.3f, 0.1f);		
	}

	public void DropInSpace(Transform tr){
		followPinch = false;
		dropped = true;
		Dim();

		targetTransform = tr;
	}

	public void Grab(){
		followPinch = true;
		dropped = false;
		Brighten();
	}
}