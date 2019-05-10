using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class PinchCursor : MonoBehaviour
{

	public Chirality whichHand;

	public bool useMinY = true;
	private Vector3 initScale;

	[HideInInspector]
	public bool shown = true;
	public Transform minYTransform;
	public HandRaycastTarget hrt;
	
	private CircleLine circle;
	private Vector3 targetPosition;
	private InteractionHand hand;
	private UIColor uiColor;
	private float minY;
	
	public bool forceHide = false;
	void Start ()
	{
		initScale = transform.localScale;
		circle = GetComponent<CircleLine>();
		hand = HandUIManager.GetHand(whichHand);
		uiColor = GetComponent<UIColor>();

		minY = transform.localPosition.y;
		
		if(this.enabled) Hide();
	}

	public void Show()
	{
		shown = true;
		uiColor.SetColor(0.3f,0.05f);

		
		forceHide = false;
	}
	
	public void Hide(bool forced = false)
	{
		shown = false;
		uiColor.SetColor(0, 0);

		if (forced)
		{
			forceHide = true;
		}

	}
	
	void LateUpdate ()
	{
		var hit = hrt.GetHit(whichHand);
				
		if(!hit.isHandHovering && shown) Hide();
		if(hit.isHandHovering && !shown && !forceHide) Show();

		
		if (hit.isHandHovering)
		{
			targetPosition = hit.point;
			circle.radius = Vector2.one * Mathf.Clamp(hand.pinchCloseness(), 0.2f, 1);

			
			Vector3 localTarget = transform.parent.InverseTransformPoint(targetPosition);

			if (minYTransform)
			{
				localTarget.y = Mathf.Clamp(localTarget.y, minYTransform.localPosition.y, float.PositiveInfinity);
			}


			transform.localPosition = localTarget;

		}
		
	}
}
