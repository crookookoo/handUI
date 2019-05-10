using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	public Transform Camera;
	public bool lockZRotation = true;
	public bool onlyYrotation = false;
	
	private float initialZRot;
	// Use this for initialization
	void Start () {
		if(Camera == null)
		Camera = GameObject.Find("VRCamera").transform;
		
		// if(Camera == null)
		// Camera = GameObject.Find("Camera").transform;

		initialZRot = transform.localEulerAngles.z;
	}
	
	// Update is called once per frame
	void Update () {

		//transform.LookAt(Camera); // backwards for some reason

		transform.rotation = Quaternion.LookRotation(-Camera.transform.position + transform.position);
		
		if(lockZRotation)
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, initialZRot);

		if (onlyYrotation)
		{
			Vector3 rot = transform.localEulerAngles;
			rot.x = 0;
			rot.z = 0;

			transform.localEulerAngles = rot;

		}
	}
}