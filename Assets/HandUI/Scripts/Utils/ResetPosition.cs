using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ResetPosition : MonoBehaviour {
	public Transform Camera;
	
	public KeyCode ResetKey;

	private float height = 0; 

	private Vector3 lastLocalPos;
	// Use this for initialization
	void Start () {
		lastLocalPos = Camera.localPosition;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(ResetKey)){
			
/*
			if(XRDevice.model.ToLower().Contains("vive")){
				Valve.VR.OpenVR.System.ResetSeatedZeroPose();
				Valve.VR.OpenVR.Compositor.SetTrackingSpace(
				Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);

			} else {
				InputTracking.Recenter();
			}
*/
			
			InputTracking.Recenter();

			transform.position = Vector3.zero;

		}
	}
}
