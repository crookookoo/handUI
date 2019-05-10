using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class GrabbyRing : HandUIItem {
    
    public UIEntity glow, outline;

    protected override void OnHoverBegin()
    {
        outline.SetColor(0.3f);
        glow.SetColor(0.6f);
    }

    protected override void OnHoverEnd()
    {
//        doubleHovered = false;
        outline.SetInitialColor();
        glow.SetInitialColor();

    }
    protected override void OnPinchBegin()
    {
        outline.SetColor(0.8f);

    }
    protected override void OnPinchEnd()
    {
        if(base.handUI.hovered) outline.SetColor(0.3f);
        else outline.SetColor(0.06f);
    }

    
}