using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RayThermostat : HandRaycastItem
{
    public RayHoverUI rayHover;
    public UIEntity lines, value;
    public TextMeshPro nestText;

    private TextMeshPro valueText;

    private float onGrabVal, val, zAngleOnGrab;
    private Vector3 anglesOnGrab;
    
    private bool isPinched;

    void Start()
    {
        valueText = value.GetComponent<TextMeshPro>();
        val = float.Parse(valueText.text);
    }

    void Update()
    {
        if (isPinched)
        {
            //watch vertical movement and adjust value
            float dY = base.hrt.hoveringRaycast.GetCurrentDragDelta().y;
            val = onGrabVal + dY * 100;
            valueText.text = val.ToString("00");
            nestText.text = val.ToString("00");
            
            //rotate lines
            Vector3 newAngles = lines.transform.localEulerAngles;
            newAngles.z = zAngleOnGrab - dY * 1000;
            lines.transform.localEulerAngles = newAngles;
        }
    }

    protected override void OnHoverBegin()
    {
        //show UI
        if(!rayHover.shown) rayHover.Appear(base.hrt.WhichHandHovering());
    }
    protected override void OnHoverEnd()
    {
        //hide UI
        rayHover.Disappear();
    }
    protected override void OnPinchBegin()
    {
        rayHover.PinchBegin();
        onGrabVal = val;
        zAngleOnGrab = lines.transform.eulerAngles.z;
        isPinched = true;
        
        lines.SetColor(0.7f);
    }
    protected override void OnPinchEnd()
    {
        rayHover.PinchEnd();
        isPinched = false;
        lines.SetInitialColor();

    }
}
