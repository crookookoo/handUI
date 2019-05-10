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
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(UIEntity))]
public class HandRaycastButton : HandRaycastItem
{

	public UIEntity fill;
	public UnityEvent ClickEvent;
	
	private UIEntity self;
	
	void Start ()
	{
		self = GetComponent<UIEntity>();
	}

	protected override void OnHoverBegin()
	{
		self.Scale(1.1f);
		if(fill) fill.SetColor(0.1f,0);
	}

	protected override void OnHoverEnd()
	{
		self.Scale(1f);
		if(fill) fill.SetColor(0);
	}

	protected override void OnPinchBegin()
	{		
		self.Punch();
		DOVirtual.DelayedCall(0.1f, () =>
		{
			ClickEvent.Invoke();			
		});

	}
	
}
