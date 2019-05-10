using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Leap.Unity.Gestures;
using Leap.Unity.Interaction;
using UnityEngine;

public class HandRaycastLine : MonoBehaviour
{
	[SerializeField] private UIEntity sphere;
	[SerializeField] private float lineLength = 0.25f;

	private HandRaycast hr;
	private LineRenderer line;
	private InteractionHand hand;
	private PinchGesture pinch;
	private bool shown = false;
	private bool pinched = false;
	void Start ()
	{
		line = GetComponent<LineRenderer>();
		if (transform.parent.GetComponent<HandRaycast>())
		{
			hr = transform.parent.GetComponent<HandRaycast>();
			hand = HandUIManager.GetHand(hr.whichHand);			
		}
		else
		{
			HandUIManager.Warn("HandRaycast not found", this);
		}
		
		DimDown();
		Hide();
	}

	private void OnEnable()
	{
		pinch = transform.parent.GetComponent<PinchGesture>();
		
	}

	void UpdateLine()
	{
		Vector3 start = hand.pinchPos();
		Vector3 direction = hr.direction;

		if (hr.hovering)
		{
			Vector3 point = hr.currentTarget.GetHit(hr.whichHand).point;
			direction = (point - start).normalized;
		}
		
		line.SetPosition(0, start);
		line.SetPosition(1, start + lineLength * direction);
	}

	void Hide()
	{
		shown = false;
		line.enabled = false;
		sphere.Hide(0, 0.01f);
	}

	void Show()
	{
		shown = true;
		line.enabled = true;
		sphere.Show(0, 0.01f);
	}

	void LightUp()
	{
		line.colorGradient.alphaKeys[0] = new GradientAlphaKey(1f, 0);

	}

	void DimDown()
	{
		line.colorGradient.alphaKeys[0] = new GradientAlphaKey(0.5f, 0);
		
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{

		if (hand.isTracked)
		{
			bool pointing = Vector3.Angle(hand.leapHand.PalmNormal.ToVector3(), hr.direction) < 70 && hr.hovering;
			
			if (shown && !pointing)
			{
				Hide();
			}

			if (!shown && pointing)
			{
				Show();
			}			
			
			if(shown) UpdateLine();

			if(pinch != null && pinch.isActive && !pinched){
				LightUp();
				pinched = true;
				sphere.Hide(0, 0.01f);
			}

			if(pinch != null && !pinch.isActive && pinched){
				DimDown();
				pinched = false;
				if(pointing) sphere.Show(0, 0.01f);
			}

		}
		else
		{
			if(shown) Hide();
		}

	}
}
