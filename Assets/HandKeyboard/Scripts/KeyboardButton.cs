using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using WindowsInput;
using Leap.Unity;

public class KeyboardButton : HandUIItem
{
	[SerializeField] private VirtualKeyCode virtualKeyCode;

	[SerializeField] private bool isSymbol;
	[SerializeField] private UIEntity letter, button;
	[SerializeField] private float maxHeight = 0.1f;
	[SerializeField] private UnityEvent OnClickEvent;

	private float hoverDist = 0.05f;
	private string character;
	private bool hovered = false;
	private UIEntity self;

	[HideInInspector] public KeyboardManager manager;

	private TextMeshPro letterTMP;
	private bool caps = false;
	void Start ()
	{

		if (letter.transform.GetComponent<TextMeshPro>())
		{
			letterTMP = letter.transform.GetComponent<TextMeshPro>();
			character = letterTMP.text;
		}

		if (character == "") character = " ";
		self = GetComponent<UIEntity>();

	}

	public void ToggleCaps()
	{
		if (isSymbol)
		{
			if (!caps) letterTMP.fontStyle = FontStyles.UpperCase;
			else 	  letterTMP.fontStyle = FontStyles.LowerCase;

			caps = !caps;
		}
	}
	void Update () {

		//look at dist to each pointer
		Vector3 pointerLPos = Vector3.zero;
		
		//if close, trigger hover state
		
		Vector3 localPosTarget = letter.GetInitPos();
		
		//if hovered watch out for pinch
		if (hovered)
		{
			localPosTarget = letter.GetInitPos() - base.GetPinchCloseness() * Vector3.forward * 10000;
		}

		letter.transform.localPosition.Follow(localPosTarget);
		button.transform.localPosition.Follow(localPosTarget);
		
		float degreeClosed = 0;


	}

	protected override void OnHoverBegin()
	{
		hovered = true;
		button.SetColor(0.05f);
//		manager.HoverCharachter(character, base.handUI.closestHand.isLeft);
//		manager.AddCharacter(character);

	}

	protected override void OnHoverEnd()
	{
		hovered = false;
		button.SetInitialColor();
	}

	public void OnPress()
	{
//		manager.AddCharacter(virtualKeyCode);

		
//		OnClickEvent.Invoke();
	}

	public void OnRelease()
	{
		
	}

	protected override void OnPinchBegin()
	{
		InputSimulator.SimulateKeyDown(virtualKeyCode);
		button.Punch();
		OnClickEvent.Invoke();

		if (caps)
		{
			manager.ToggleCaps();
			InputSimulator.SimulateKeyDown(VirtualKeyCode.CAPITAL);

		}
		
//		button.Scale(0.9f,0.01f);
//		base.OnPinch();
//		manager.AddCharacter(virtualKeyCode);
	}

	protected override void OnPinchEnd()
	{
		InputSimulator.SimulateKeyUp(virtualKeyCode);

	}

}
