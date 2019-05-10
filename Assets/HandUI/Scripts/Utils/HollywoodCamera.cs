using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollywoodCamera : MonoBehaviour {
	public Transform Head;
	public bool lockToHead = false;
	public bool steady = false; 
	public bool fixRoll = false;

	private float positionSmoothing = 0.5f;
	private float rotationSmoothing = 0.05f;

	private Camera camera;
	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
		camera.enabled = false;
	}
	
	public void ToggleCamera(){
		camera.enabled = !camera.enabled;
	}

	// Update is called once per frame
	void Update () {
		if(lockToHead){
			if(!steady){
				transform.position = Head.position;
				transform.rotation = Head.rotation;
			} else {
				float angle = Quaternion.Angle(transform.rotation, Head.rotation);
				transform.position += (Head.position - transform.position) * 0.5f * Time.deltaTime * 100;
				transform.rotation =  Quaternion.RotateTowards(transform.rotation, Head.rotation,  angle * rotationSmoothing * Time.deltaTime * 100 );

			}

			if(fixRoll){
				Vector3 angles = transform.eulerAngles;
				angles.z = 0;
				transform.eulerAngles = angles;
			}
		}
	}
}
