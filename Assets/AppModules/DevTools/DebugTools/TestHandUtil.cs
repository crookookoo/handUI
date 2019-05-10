using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestHandUtil {

  public static Hand MakeTestHand(bool isLeft) {
    Hand hand;
    if (isLeft) {
      hand = TestHandFactory.MakeTestHand(isLeft: true);
      hand.TransformToUnityUnits(); // Note: sucks that this has to be here
    }
    else {
      hand = TestHandFactory.MakeTestHand(isLeft: false);
      hand.TransformToUnityUnits(); // Note: like really this is terrible
    }

    return hand;
  }

}

// Note: The fact that this class needs to exist is ridiculous
public static class LeapTestProviderExtensions {

  public static readonly float MM_TO_M = 1e-3f;

  public static LeapTransform GetLeapTransform(Vector3 position, Quaternion rotation) {
    Vector scale = new Vector(MM_TO_M, MM_TO_M, MM_TO_M); // Leap units -> Unity units.
    LeapTransform transform = new LeapTransform(position.ToVector(), rotation.ToLeapQuaternion(), scale);
    transform.MirrorZ(); // Unity is left handed.
    return transform;
  }

  public static void TransformToUnityUnits(this Hand hand) {
    hand.Transform(GetLeapTransform(Vector3.zero, Quaternion.identity));
  }

}