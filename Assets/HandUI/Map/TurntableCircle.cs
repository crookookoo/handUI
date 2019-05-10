using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurntableCircle : MonoBehaviour {

	public Transform bottom;

	public bool hideOnStart = true;
	public bool scale = true;
	public float scaleMultiplier = 1.25f;

	private UIColor uIColor;
	private float initLocalZ;

	private bool shown = false;
	private float yOffset = 0.03f;

	public void Show(){
		transform.DOLocalMoveY(-yOffset,0.1f);
		if(scale) transform.DOScale(bottom.parent.localScale * scaleMultiplier,0.1f);		
		uIColor.SetColor(0.15f);
		shown = true;
	}

	public void Hide(){
		transform.DOLocalMoveY(0 ,0.1f);
		if(scale) transform.DOScale(bottom.parent.localScale,0.1f);		
		uIColor.SetColor(0);
		shown = false;

	}

	// Use this for initialization
	void Start () {
		uIColor = transform.GetComponent<UIColor>();
		if(hideOnStart) Hide();
	}
	
	// Update is called once per frame
	void Update () {
		if(bottom.parent.hasChanged){
			if(scale) transform.localScale = bottom.parent.localScale * (1 + (scaleMultiplier - 1) * (shown ? 1 : 0));
			transform.localPosition = - yOffset * (shown ? 1 : 0) * Vector3.up;
		}
	}
}
