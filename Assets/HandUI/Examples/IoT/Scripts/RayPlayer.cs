using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RayPlayer : HandRaycastItem
{
    public RayHoverUI rayHover;

    public SimplePlayer player;

    private bool justCollapsed;

    private float collapseTimer = 0;
    private float collapseTimeThreshold = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (justCollapsed)
        {
            collapseTimer += Time.deltaTime;
            if (collapseTimer > collapseTimeThreshold)
            {
                justCollapsed = false;
                collapseTimer = 0;
            }
        }
    }
    protected override void OnHoverBegin()
    {
        //show UI
        if(!rayHover.shown && !justCollapsed) rayHover.Appear(base.hrt.WhichHandHovering());
    }
    protected override void OnHoverEnd()
    {
        //hide UI
        if(!player.expanded)
            rayHover.Disappear();
    }
    protected override void OnPinchBegin()
    {
        rayHover.PinchBegin();
        rayHover.SetFreeze(true);
        
        player.Expand();
        player.GetComponent<Billboard>().enabled = false;
        base.hrt.GetComponent<Collider>().enabled = false;
        player.GetComponent<Collider>().enabled = true;
    }

    public void ClosePlayer()
    {
        player.Collapse();

        DOVirtual.DelayedCall(0.1f, () =>
        {
            player.GetComponent<Billboard>().enabled = true;
            base.hrt.GetComponent<Collider>().enabled = true;
            player.GetComponent<Collider>().enabled = false; 
            rayHover.SetFreeze(false);
            rayHover.Disappear();
            justCollapsed = true;
        });


    }
    protected override void OnPinchEnd()
    {
        rayHover.PinchEnd();
    }

    protected override void OnPinchClick()
    {
    }

}
