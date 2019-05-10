using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Leap.Unity;
using Leap.Unity.Interaction;
using UnityEngine;

public class RayHoverUI : MonoBehaviour
{
    public Vector3 pinchPointOffsetR;
    // Start is called before the first frame update
    private Vector3 pinchPointOffsetL;
    public bool freezeOnPinch = false;
    
    private Vector3 currentOffset;
    private UIEntity self;
    [HideInInspector] public bool shown = false;
    private Chirality whichHandHovering;
    private InteractionHand currentlyHoveringHand;
    private bool frozen = false;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position + pinchPointOffsetR, transform.position + pinchPointOffsetR + Vector3.left * 0.003f);
        Gizmos.DrawSphere(transform.position + pinchPointOffsetR,0.001f);
        
        Vector3 l = pinchPointOffsetR;
        l.x *= -1;
        Gizmos.DrawLine(transform.position + l, transform.position + l + Vector3.right * 0.003f);
        Gizmos.DrawSphere(transform.position + l,0.001f);

    }

    void Start()
    {
        self = GetComponent<UIEntity>();
        self.Hide();

        pinchPointOffsetL = pinchPointOffsetR;
        pinchPointOffsetL.x *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (shown && !frozen)
        {
            Transform pinch = currentlyHoveringHand.stablePinchTransform();
            transform.position = pinch.TransformPoint(-currentOffset);
        }
    }

    public void Appear(Chirality whichHand)
    {
        self.Show(0, 0.1f);
        currentlyHoveringHand = HandUIManager.GetHand(whichHand);
        currentOffset = whichHand == Chirality.Right ? pinchPointOffsetR : pinchPointOffsetL;
        shown = true;
    }

    public void Disappear()
    {
        self.Hide(0, 0.05f);
        shown = false;
    }

    public void SetFreeze(bool value)
    {
        frozen = value;
        transform.DOMove(currentlyHoveringHand.stablePinchTransform().TransformPoint(-currentOffset), 0.4f);
    }
    public void PinchBegin()
    {
        if (freezeOnPinch) frozen = true;
    }

    public void PinchEnd()
    {
        if (freezeOnPinch) frozen = false;
        
    }
    
    
}
