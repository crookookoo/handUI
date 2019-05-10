using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WindowsInput;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardManager : MonoBehaviour
{

	public TMP_InputField inputField;
	public Transform pointerL, pointerR;
	public Vector3 spawnOffset;
	public HandRaycastTarget raycastTarget;
	
	private List<KeyboardButton> buttons;

	private KeyboardButton hoveredRButton, hoveredLButton;
	
	private float hoverDist = 0.015f;
	private UIEntity self;

	private bool backspacePressed = false;

	private float bspTimer = 0;
	// Use this for initialization
	void Start ()
	{
		buttons = GetComponentsInChildren<KeyboardButton>().ToList();

		foreach (var button in buttons)
		{
			button.manager = this;
		}
		
		self = GetComponent<UIEntity>();
		
	}

	private void OnEnable()
	{
		raycastTarget.OnHoverBegin += OnHover;
		raycastTarget.OnHoverEnd += OnUnhover;
	}

	private void OnDisable()
	{
		raycastTarget.OnHoverBegin -= OnHover;
		raycastTarget.OnHoverEnd -= OnUnhover;
	}

	private void OnHover()
	{
		EventSystem.current.SetSelectedGameObject(inputField.gameObject);

	}

	private void OnUnhover()
	{
		EventSystem.current.SetSelectedGameObject(null);
		
	}

	// Update is called once per frame
	void Update ()
	{

		if (backspacePressed)
		{
			bspTimer += Time.deltaTime;
			if (bspTimer > 0.2f)
			{
				if (inputField.text.Length > 0)
				{
					inputField.text = inputField.text.Remove(inputField.text.Length - 1);
					inputField.caretPosition -= 1;				
				}
			}
		}
	}

	public void AddCharacter(VirtualKeyCode virtualKeyCode)
	{
		InputSimulator.SimulateKeyPress(virtualKeyCode);
	}

	
	public void SpawnKeyboard()
	{
		transform.localScale = Vector3.zero;
		
		transform.position = HandUIManager.PostitionInFrontOfHead(spawnOffset.z, spawnOffset.y);
		transform.LookAt(transform.position - (HandUIManager.head.position - transform.position));
		transform.eulerAngles = Vector3.up * transform.eulerAngles.y;

		self.Scale(1);
		
//		Vector3 newPos = HandUIManager.PostitionInFrontOfHead() ;

	}

	private void LateUpdate()
	{
		
	}

	public void BackspaceToggle(bool pressed)
	{
		backspacePressed = pressed;
		bspTimer = 0;
	}

	public void ClearInputField()
	{
		inputField.Select();
		inputField.text = "";
	}

	public void ToggleCaps()
	{
		foreach (var item in buttons)
		{
			item.ToggleCaps();
		}
	}
	
}
