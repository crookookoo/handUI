using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUIItem : MonoBehaviour {
	
	[HideInInspector] public HandUIObject handUI;

	private bool isPinched = false;
	
	void OnEnable()
	{
		handUI = transform.GetComponent<HandUIObject>();
		if(handUI == null) HandUIManager.Warn("HandUIItem couldn't find HandUIObject!", this);
		
		handUI.OnHoverBegin += OnHoverBegin;
		handUI.OnHoverEnd += OnHoverEnd;
		handUI.OnPinchBegin += OnPinchBegin;
		handUI.OnPinchEnd += OnPinchEnd;
		
	}

	private void OnDisable() {
		handUI.OnHoverBegin -= OnHoverBegin;
		handUI.OnHoverEnd -= OnHoverEnd;
		handUI.OnPinchBegin -= OnPinchBegin;
		handUI.OnPinchEnd -= OnPinchEnd;
	}


	protected virtual void OnHoverBegin(){
		
	}

	protected virtual void OnHoverEnd(){
		
	}

	protected virtual void OnPinchBegin(){
		
	}

	protected virtual void OnPinchEnd(){
		
	}
	
	public float GetPinchCloseness()
	{
		return handUI.closestHand.pinchCloseness();
	}

	
	
}
