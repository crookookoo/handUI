using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;

public class LockAtPinchPosition : MonoBehaviour {
	public Chirality whichHand;
	private InteractionHand hand;

	// Use this for initialization
	void Start () {

		if(whichHand == Chirality.Left){
			hand = HandUIManager.handLeft;
		} else {
			hand = HandUIManager.handRight;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hand.isTracked){
			transform.position = hand.pinchPos();
		}
	}
}