using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Leap.Unity;
using Leap.Unity.Interaction;
using Leap.Unity.Gestures;

public class MapControlPoint : MonoBehaviour {
	public Transform map, sphere, controlPoint, bezierControl;
	public Chirality whichHand;

	private float r, R, h1, h2;
	private LineProjector line;
	[HideInInspector]
	public HandLocation handLocation = HandLocation.Close;
	private PinchGesture pg;

	private Vector3 initSphereParentScale;
	private UIColor lineColor, projectorColor;
	private Projector projector;

	private float minExt = 0.06f;
	private float maxExt = 0.4f;

	private InteractionHand hand;

	[Range(0,1)]
	public float extensionLength = 0.5f;

	private bool forceDirect = false;

	public enum HandLocation
	{
		Far,
		Close
	}

	private bool sphereShown = false;
	private bool spherePinched = false;

	private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		line = GetComponentInChildren<LineProjector>();
		lineColor = line.transform.GetComponent<UIColor>();
		pg = HandUIManager.GetPinchGesture(whichHand);
		hand = whichHand == Chirality.Left ? HandUIManager.handLeft : HandUIManager.handRight;

		projector = GetComponentInChildren<Projector>();
		projectorColor = projector.transform.GetComponent<UIColor>();

		initSphereParentScale = sphere.parent.localScale;

		CalculateBounds();

		HideSphere();
	}
	
	void PositionContolPoint(){
		Vector3 endPos = transform.position;
		endPos.y = map.position.y;
		
		Vector3 shoulderPosition = whichHand == Chirality.Left ? HandUIManager.head.TransformPoint(new Vector3(-.1f, -.1f, 0f)) : HandUIManager.head.TransformPoint(new Vector3(.1f, -.1f, 0f));
		float ext = shoulderPosition.XZSqrDist(pg.pose.position);

		Vector3 XZdir = pg.pose.position - shoulderPosition;
		XZdir.y = 0;

		float heightFactor = pg.pose.position.y - map.position.y;
		heightFactor = Mathf.InverseLerp(0.15f, 0.65f, heightFactor);

		controlPoint.position = endPos + XZdir * Mathf.InverseLerp(minExt, maxExt, ext) * heightFactor * extensionLength * 6 * (forceDirect ? 0 : 1);

		// Vector3 bc = controlPoint.position;
		// bc.y += (pg.pose.position.y - controlPoint.position.y) * 0.75f;
		
		Vector3 diff = controlPoint.position - transform.position;
		Vector3 diffXZ = diff;
		diffXZ.y = 0;

		line.bPoint2 = controlPoint.position - diff.y * Vector3.up - diffXZ * 0.15f;
		line.bPoint1 = transform.position + diffXZ * 0.5f;
		// bezierControl.position = bc;

	}


	private void WatchHandLocation(){
		float d = transform.position.XZSqrDist(map.position);

		if(hand.isTracked && d < R * R && transform.position.y < h2){
			SetHandLocation(HandLocation.Close);
		} else {
			SetHandLocation(HandLocation.Far);
		}
	}

	void SetHandLocation(HandLocation loc){
		if(loc != handLocation) {
			// if(handLocation == HandLocation.Far) ShowSphere();
			// if(loc == HandLocation.Far) HideSphere();
			//location has changed
			handLocation = loc;
			

			switch(handLocation){
				case HandLocation.Far:
					line.Hide();
					HideSphere();
				break;
				case HandLocation.Close:
					ShowSphere();
					line.Show();
				break;
				default:
				break;
			}

		} else return;
	}

	void ShowSphere(){
		sphere.DOScale(Vector3.one, 0.1f);
		sphereShown = true;
		DOTween.To(()=> projector.orthographicSize, x=> projector.orthographicSize = x, 0.025f, 0.1f);

	}

	void HideSphere(){
		sphere.DOScale(Vector3.zero, 0.1f);
		sphereShown = false;	

		DOTween.To(()=> projector.orthographicSize, x=> projector.orthographicSize = x, 0, 0.1f);
	
	}

	void PinchSphere(){
		sphere.parent.DOScale(Vector3.zero, 0.1f);
		spherePinched = true;
		lineColor.SetColor(1);
		projectorColor.SetColor(1);


		DOTween.To(()=> projector.orthographicSize, x=> projector.orthographicSize = x, 0.035f, 0.1f);

		// projector.orthographicSize.tweenTo(0.4f);

	}

	void UnpinchSphere(){
		sphere.parent.DOScale(initSphereParentScale, 0.04f);
		spherePinched = false;
		lineColor.SetColor(0.5f);
		projectorColor.SetColor(projectorColor.startOpacity);

		DOTween.To(()=> projector.orthographicSize, x=> projector.orthographicSize = x, 0.025f, 0.1f);

		// projector.orthographicSize.tweenTo(0.3f);
	}

	void CalculateBounds(){
		r = map.localScale.x;
		R = map.GetChild(0).localScale.x;
		h1 = 0.25f;
		h2 = 0.5f;
	}

	// Update is called once per frame
	void Update () {
		if(!hand.isTracked && handLocation != HandLocation.Far){
			SetHandLocation(HandLocation.Far);
		}

		PositionContolPoint();

		if(map.hasChanged){
			CalculateBounds();
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			forceDirect = !forceDirect;
		}

		if(!spherePinched && pg.isActive && handLocation == HandLocation.Close) PinchSphere();
		if(spherePinched && !pg.isActive && handLocation == HandLocation.Close) UnpinchSphere();


		WatchHandLocation();
	}
}
