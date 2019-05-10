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

public class TabKeyCommand : MonoBehaviour {
	public KeyCode tabPlus;
	public UnityEvent OnPress;

	private bool tabPressed = false;

	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Tab)) tabPressed = true;
		if(Input.GetKeyUp(KeyCode.Tab)) tabPressed = false;

		if(tabPressed && Input.GetKeyDown(tabPlus)){
			OnPress.Invoke();
		}
	
	}
}
