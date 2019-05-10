using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotationToTransform : MonoBehaviour {

	public Transform objectToLockTo;

	// Use this fo	r initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = objectToLockTo.rotation;
	}
}
