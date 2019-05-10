using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RaySlider : HandUIItem
{

    public SliderOrientation orientation;
    public UIEntity sliderBg, sliderFilled, sliderOutline, sliderCenter;

    private LineRenderer sliderBgLine, sliderFilledLine;
    private Vector3 localLimitA, localLimitB, initLocalPos, initBarPos;
    private float fullLength;
   

    public enum SliderOrientation
    {
        Horizontal,
        Vertical
    }
    
    void Start()
    {
        sliderBgLine = sliderBg.GetComponent<LineRenderer>();
        sliderFilledLine = sliderFilled.GetComponent<LineRenderer>();

        localLimitA = sliderBgLine.GetPosition(0);
        localLimitB = sliderBgLine.GetPosition(1);

        initLocalPos = transform.localPosition;
        initBarPos = sliderFilledLine.GetPosition(1);

        fullLength = orientation == SliderOrientation.Vertical ? localLimitB.x : localLimitB.y;
        
        SetFill(GetValue());
    }

    void LateUpdate()
    {
        // Apply limits
        Vector3 locPos = transform.localPosition;

        locPos.z = initLocalPos.z;
            
        if (orientation == SliderOrientation.Vertical)
        {
            locPos.x = initLocalPos.x;
            locPos.y = Mathf.Clamp(locPos.y,localLimitA.y, localLimitB.y);

        }
        else
        {
            locPos.y = initLocalPos.y;
            locPos.x = Mathf.Clamp(locPos.x,localLimitA.x, localLimitB.x);
        }

        transform.localPosition = locPos;
        
        SetFill(GetValue());

    }

    void SetFill(float value)
    {
        
        if (orientation == SliderOrientation.Vertical)
        {
            float pos = Mathf.Lerp(localLimitA.y, localLimitB.y, value);
            sliderFilledLine.SetPosition(1, new Vector3(initBarPos.x, pos, initBarPos.z));
        }
        else
        {
            float pos = Mathf.Lerp(localLimitA.x, localLimitB.x, value);
            sliderFilledLine.SetPosition(1, new Vector3(pos, initBarPos.y, initBarPos.z));
        }

    }

    public float GetValue()
    {
        if (orientation == SliderOrientation.Vertical)
        {
            return Mathf.InverseLerp(localLimitA.y, localLimitB.y, transform.localPosition.y);
        }
        else
        {
            return Mathf.InverseLerp(localLimitA.x, localLimitB.x, transform.localPosition.x);
        }
    }

    public void SetValue(float value)
    {        
        if (orientation == SliderOrientation.Vertical)
        {
            float pos = Mathf.Lerp(localLimitA.y, localLimitB.y, value);
            transform.DOLocalMoveY(pos, 0.1f);
        }
        else
        {
            float pos = Mathf.Lerp(localLimitA.x, localLimitB.x, value);
            transform.DOLocalMoveX(pos, 0.1f);
        }
    }

    protected override void OnHoverBegin()
    {
        sliderFilled.SetColor(0.5f);
        sliderOutline.SetColor(0.5f);

        sliderOutline.Scale(1.4f);
        sliderCenter.Scale(1.4f);

    }
    
    protected override void OnHoverEnd()
    {
        sliderFilled.SetInitialColor();
        sliderOutline.SetInitialColor();
        
        sliderOutline.Scale(1);
        sliderCenter.Scale(1);

    }
    protected override void OnPinchBegin()
    {
        sliderFilled.SetColor(1f);
        sliderOutline.SetColor(1f);
    }

    protected override void OnPinchEnd()
    {
        if (base.handUI.hovered)
        {
            OnHoverBegin();
        }
        else
        {
            OnHoverEnd();
        }
    }

}
