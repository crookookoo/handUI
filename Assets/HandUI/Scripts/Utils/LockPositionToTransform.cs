using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPositionToTransform : MonoBehaviour {

	public Transform objectToLockTo;
	public bool lazyFollow = false;
	[Range(0,1)]
	public float followSpeed = 1;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!lazyFollow)
			transform.position = objectToLockTo.position;
		else
		{
			transform.position += (objectToLockTo.position - transform.position) * followSpeed * 100 * Time.deltaTime;
		}
	}
}
