using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Gestures;
using Leap.Unity.Interaction;
using UnityEngine;

public class KeyboardPointer : MonoBehaviour
{
	public float vericalOffset = 10;
	public Transform keyboardPlane;
	public Chirality whichHand;

	private InteractionHand hand;
	private PinchGesture pinch;
	
	private float xLimit = 10.5f;
	private float yLimit = 3.5f;
	private HandRaycastTarget hrt;
	private UIEntity self;

	private bool shown;
	
	private void OnEnable()
	{
		hand = HandUIManager.GetHand(whichHand);
		pinch = HandUIManager.GetPinchGesture(whichHand);
		self = GetComponent<UIEntity>();
		
		hrt = keyboardPlane.GetComponent<HandRaycastTarget>();
		
		pinch.OnOpen += OnPinch;
		self.Hide(0,0.05f);

	}

	private void OnDisable()
	{
		pinch.OnOpen -= OnPinch;
	}

	void LateUpdate ()
	{
	
		float d = hand.pinchCloseness();
		
		Vector3 newPos = Vector3.zero;

		var hit = hrt.GetHit(whichHand);
		
		if (hit.isHandHovering)
		{
			transform.position = hit.point;
			transform.localScale = self.GetInitScale() * Mathf.Clamp(hand.pinchCloseness(), 0.2f, 1);
			if (!shown)
			{
				self.Show(0,0.05f);
				shown = true;
			}
		}
		else
		{
			if (shown)
			{
				self.Hide(0,0.05f);
				shown = false;
			}
		}
		

	}

	void OnPinch()
	{
		
	}
}
