/******************************************************************************
 
 Copyright (C) 2019 Eugene Krivoruchko                                           

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 ******************************************************************************/

using UnityEngine;

[RequireComponent(typeof(HandRaycastTarget))]
public class HandRaycastItem : MonoBehaviour
{	
    [HideInInspector] public HandRaycastTarget hrt;
    [HideInInspector] public bool pinched = false;
    [HideInInspector] public bool hovered = false;
    private float tPinch = 0;
    private float tHover = 0;
    
    private void OnEnable()
    {
        hrt = GetComponent<HandRaycastTarget>();
        hrt.OnHoverBegin += OnHoverBegin;
        hrt.OnHoverEnd += OnHoverEnd;
        hrt.OnPinchBegin += OnPinchBegin;
        hrt.OnPinchEnd += OnPinchEnd;
    }
	
    private void OnDisable()
    {
        hrt.OnHoverBegin -= OnHoverBegin;
        hrt.OnHoverEnd -= OnHoverEnd;
        hrt.OnPinchBegin -= OnPinchBegin;
        hrt.OnPinchEnd -= OnPinchEnd;
    }

    protected virtual void OnHoverBegin()
    {
        hovered = true;
    }

    protected virtual void OnHoverEnd()
    {
        hovered = false;
    }

    protected virtual void OnPinchBegin()
    {
        pinched = true;
    }

    protected virtual void OnPinchEnd()
    {
        if (tPinch < 0.1f)
        {
            OnPinchClick();
        }

        pinched = false;
        tPinch = 0;
    }

    protected virtual void OnPinchClick()
    {
      
    }

    private void Update()
    {
        if (pinched) tPinch += Time.deltaTime;

        if (hovered && !pinched) tHover += Time.deltaTime;
    }
}
