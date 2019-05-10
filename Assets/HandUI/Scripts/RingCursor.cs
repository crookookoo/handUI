using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class RingCursor : MonoBehaviour
{
    public Chirality whichHand;
    public HandUIObject handUI;
    public UIEntity sphere;
    public float lineLength = 0.05f;
    
    private LineRenderer lineRend;
    private InteractionHand hand;
    private bool shown = false;
    
    private void Start()
    {
        hand = HandUIManager.GetHand(whichHand);
        Hide();
    }

    private void LateUpdate()
    {
        bool isHandClose = handUI.isHandClose(hand);

        bool isGrabbedByOtherHand = handUI.pinched && handUI.closestHand == hand.otherHand();
        bool isTwoHandedAllowed = handUI.twoHandedTRS;

        if (isTwoHandedAllowed)
        {
            if (!shown && isHandClose && !hand.isPinched())
            {
                Show();
                shown = true;
            }

            if (shown && (!isHandClose || hand.isPinched()))
            {
                Hide();
                shown = false;
            }             
        }
        else
        {
            if ((!isHandClose || isGrabbedByOtherHand || hand.isPinched()) && shown)
            {
                Hide();
                shown = false;               
            }

            if (!isGrabbedByOtherHand && !shown && isHandClose && !hand.isPinched())
            {
                Show();
                shown = true;
            }
        }
        
        if (shown)
        {
            transform.position = handUI.ClosestPointOnCircleToHand(hand);
        }
    }

    void Show()
    {
        sphere.Show();
    }

    void Hide()
    {
        sphere.Hide(0, 0.05f);
    }
}
