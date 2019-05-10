using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionBetween : MonoBehaviour {
	public Transform pointA, pointB;
	public Vector3 offset = Vector3.zero;
	[Range(0,1)]
	public float t = 0.3f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = Vector3.Lerp(pointA.position, pointB.position, t) + offset;
	}
}
