using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarFollow : MonoBehaviour {
	public Transform target;
	public enum Plane {
		XY,
		YZ
	};

	public Plane plane;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 localTarget = transform.InverseTransformPoint(target.position);

		if(plane == Plane.XY)
			localTarget.z = transform.localPosition.z;
		else if(plane == Plane.YZ)
			localTarget.x = transform.localPosition.x;			

		transform.localPosition = localTarget;

	}
}
