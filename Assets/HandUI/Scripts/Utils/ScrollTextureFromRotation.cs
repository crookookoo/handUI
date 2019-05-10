using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTextureFromRotation : MonoBehaviour {

	public Transform objectToWatch;
	public float multiplier = 1f;

	private float prevY = 0;
	private float textureXOffset;
	private LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
		textureXOffset = lineRenderer.material.GetTextureOffset("_MainTex").x;

		prevY = objectToWatch.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		float delta = objectToWatch.eulerAngles.y - prevY;

		if(Mathf.Abs(delta) > 0){
			textureXOffset += delta * multiplier * 100;
			lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(textureXOffset,0));

		}

		prevY = objectToWatch.eulerAngles.y;
	}
}
