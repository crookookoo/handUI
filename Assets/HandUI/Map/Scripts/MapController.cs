using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public MapControlPoint rightControlPoint, leftControlPoint;
	public Transform rPoint, lPoint;
	public Vector2 scaleRange;

	public bool adjustHeightFromHeightMap = false;
	public Texture2D heightMap;

	public float oneAxisLimitAtScaleOne = 1.55f; // essentially mesh width/2

	private bool reset = false;

	private bool isDoublePinched = false;

	private PinchState pinchState = PinchState.None;

	private float deltaAngle, prevAngle, onGrabDistanceBetweenHands;

	private Vector3 prevMidpointPos, onGrabScale, onGrabEulerAngles, onGrabMidpointOffset, onGrabRightToLeft, deltaPos, prevPointPos;

	private float textureWidth;
	private float initalY;

	private enum PinchState {
		None,
		Right,
		Left,
		Both
	}

	// Use this for initialization
	void Start () {
		deltaPos = Vector3.zero;

		textureWidth = heightMap.width;
		initalY = transform.localPosition.y;
	}
	
	void ResetLinearMotion(Transform point){
		deltaPos = Vector3.zero;
		prevPointPos = point.position;

		if(Mathf.Abs(deltaAngle) < 1) deltaAngle = 0;
	}

	void MoveAlongWith(Transform point){
		deltaPos = point.position - prevPointPos;
		deltaPos.y = 0;

		transform.position += deltaPos;

		prevPointPos = point.position;

	}

	void ApplyLimits(){
		float maxSideDist = transform.localScale.x * oneAxisLimitAtScaleOne;
		float maxDistSqr = maxSideDist * maxSideDist / 4;

		//apply force towards center
		Vector3 force = - transform.localPosition.normalized;
		transform.localPosition += Mathf.Clamp01(transform.localPosition.sqrMagnitude - maxDistSqr) * force * 10 * Time.deltaTime;

	}


	void ApplyHeightAdjustment(){

		//find reltaive center coords

		float currentFullWorldWidth = 2 * 15.35f * transform.localScale.x;
		
		Vector2 lowerLeft = new Vector2(-currentFullWorldWidth/2, -currentFullWorldWidth/2);

		float xPercent =  transform.localPosition.x / currentFullWorldWidth;
		float yPercent =  transform.localPosition.z / currentFullWorldWidth;

		Vector2 centralPos = new Vector2(xPercent, yPercent);//.Rotate( transform.eulerAngles.y);

		Vector2 center = (new Vector2(0.5f - centralPos.x, 0.5f - centralPos.y)) * textureWidth;
        
		int circleRadius = (int) Mathf.Floor(1 / currentFullWorldWidth * textureWidth);

		Vector2[] points = new Vector2[8];

		float[] heights = new float[8];

		// circleRadius = 0;

		for(int i = 0; i < heights.Length; i++){
			Vector2 point = center + Vector2.up.Rotate(i * 360 / heights.Length) * circleRadius;
			point.x = Mathf.Floor(point.x);
			point.y = Mathf.Floor(point.y);
			heights[i] = heightMap.GetPixel((int)point.x, (int)point.y).grayscale;
		}

		float min = Mathf.InverseLerp(0.05f, 0.9f, Mathf.Min(heights));  // 0 - 1
		// float min = Mathf.InverseLerp(0.05f, 0.9f, Mathf.Min(heights));  // 0 - 1

		float hegihtAdj = - 0.6f * min * transform.localScale.x;

		Vector3 pos = transform.localPosition;
		pos.y = initalY + hegihtAdj;

		transform.localPosition += (pos - transform.localPosition) * 0.1f * 100 * Time.deltaTime;


	}

	// Update is called once per framesdf
	void Update () {
		
		if(adjustHeightFromHeightMap) ApplyHeightAdjustment();

		if(deltaPos.sqrMagnitude > 0 && pinchState == PinchState.None){
		    deltaPos = Vector3.Lerp(deltaPos * 0.93f, Vector3.zero, Time.deltaTime);

			// deltaPos *= 0.99f;
			transform.position += deltaPos;
			ApplyLimits();
		}

		if(Mathf.Abs(deltaAngle) > 0 && pinchState == PinchState.None){
			
			deltaAngle = Mathf.Lerp(deltaAngle * 0.97f, 0, Time.deltaTime);
			transform.parent.Rotate(deltaAngle * Vector3.up);

		}

		if(rightControlPoint.handLocation != MapControlPoint.HandLocation.Far && 
			leftControlPoint.handLocation != MapControlPoint.HandLocation.Far &&
			HandUIManager.pinchLeft.isActive && HandUIManager.pinchRight.isActive){

			// two handed control
			Vector3 midpoint = (lPoint.position + lPoint.position)/2;

			if(pinchState != PinchState.Both){

				pinchState = PinchState.Both;

				deltaPos = Vector3.zero;

				onGrabDistanceBetweenHands = Vector3.Distance(lPoint.position, rPoint.position);
				onGrabScale = transform.localScale;
				onGrabEulerAngles = transform.eulerAngles;

				prevMidpointPos = midpoint;
				deltaAngle = 0;
				reset = true;

				onGrabMidpointOffset = midpoint - transform.position;
				onGrabRightToLeft = lPoint.position - rPoint.position;
				// onGrabRightToLeft.y = 0;

			}


			float currentDistance = Vector3.Distance(lPoint.position, rPoint.position);
			Vector3 currentRightToLeft = lPoint.position - rPoint.position;
			currentRightToLeft.y = 0;

			float angle = Vector3.SignedAngle(currentRightToLeft, onGrabRightToLeft, Vector3.up);
			if(!reset) deltaAngle = prevAngle - angle; 
			else reset = false;

			prevAngle = angle;
			onGrabMidpointOffset = Quaternion.Euler(0, deltaAngle, 0) * onGrabMidpointOffset;
			transform.parent.Rotate(deltaAngle * Vector3.up);
			
			Vector3 pos = transform.position;
			Vector3 target = midpoint - onGrabMidpointOffset * currentDistance / onGrabDistanceBetweenHands;

			target.y = pos.y;

			transform.position = target;
			transform.localScale = Vector3.one * Mathf.Clamp(onGrabScale.x * currentDistance / onGrabDistanceBetweenHands, scaleRange.x, scaleRange.y);

			prevMidpointPos = midpoint;


		} else if(rightControlPoint.handLocation != MapControlPoint.HandLocation.Far &&
			HandUIManager.pinchRight.isActive){
			
			//right point only
			if(pinchState != PinchState.Right){
				pinchState = PinchState.Right;
				ResetLinearMotion(rPoint);
			}

			MoveAlongWith(rPoint);

		} else if(leftControlPoint.handLocation != MapControlPoint.HandLocation.Far &&
			HandUIManager.pinchLeft.isActive){
			
			//left point only
			if(pinchState != PinchState.Left){
				pinchState = PinchState.Left;
				ResetLinearMotion(lPoint);
			}

			MoveAlongWith(lPoint);

			// prevPointPos = lep

		} else {
			if(pinchState != PinchState.None) pinchState = PinchState.None;
		}
	}
}
